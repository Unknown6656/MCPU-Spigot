module MCPUCompiler.Core.Parser

open System.Collections.Generic
open SyntaxTree


type FunctionTableEntry =
    {
        ReturnType : TypeSpec;
        ParameterTypes : VariableType list;
        IsInlined : bool;
    }
   
let EntryPointName = "main"
let (!/) t = { Type = t; IsArray = false }
let (|Scalar|_|) (t : VariableType) = if t.IsArray then None else Some t.Type
let (|Sbool|_|) (t : VariableType) = if t = !/Bool then Some () else None
let (|Sint|_|) (t : VariableType) = if t = !/Int then Some () else None
let DeclarationType : VariableDeclaration -> VariableType = fst >> (!/)
let BuildInFunctions : (BuildInFunctions * FunctionTableEntry) list =
    let (/-->) p r =
        {
            IsInlined = true
            ReturnType = r
            ParameterTypes = [ for x in p -> { Type = x; IsArray = false } ]
        }
    [
        Func__printi, [Int] /--> Void
        Func__abs, [Int] /--> Int
        Func__min, [Int; Int] /--> Int
        Func__max, [Int; Int] /--> Int
        Func__sign, [Int] /--> Int
        Func__pow, [Int; Int] /--> Int
        // Func__log, [Int] /--> Int
        // Func__loge, [Int] /--> Int
        Func__log2, [Int] /--> Int
        Func__log10, [Int] /--> Int
        Func__iodir, [Int; Bool] /--> Void
        // Func__exp, [Int] /--> Int
    ]

type internal SymbolScope(parent : SymbolScope option) =
    let mutable list = List.empty<VariableDeclaration>

    let declaresIdentifier (identifierRef : IdentifierRef) declaration =
        snd declaration = identifierRef.Identifier

    member x.AddDeclaration decl =
        if List.exists (fun x -> snd x = snd decl) list then
            raise <| VariableAlreadyDefined (snd decl)
        else
            list <- decl :: list

    member x.FindDeclaration id =
        let found = List.tryFind (declaresIdentifier id) list
        match found with
        | Some d -> d
        | None -> match parent with
                  | Some ss -> ss.FindDeclaration id
                  | None -> raise <| NameDoesNotExist id.Identifier

type internal SymbolScopeStack() =
    let stack = Stack<SymbolScope>()
    do
        stack.Push(SymbolScope None)

    member x.CurrentScope = stack.Peek()

    member x.Push() = x.CurrentScope
                      |> Some
                      |> SymbolScope
                      |> stack.Push
   
    member x.Pop() = stack.Pop() |> ignore
    
    member x.AddDeclaration declaration = stack.Peek().AddDeclaration declaration

type FunctionTable(program) as self =
    inherit Dictionary<Identifier, FunctionTableEntry>()

    let rec containscall =
        let rec scanBody l = (List.filter (function
                                           | CompoundStatement(_, s) -> scanBody s
                                           | IfStatement(s, c, None) -> scanExpr s || scanBody [c]
                                           | IfStatement(s, c1, Some c2) -> scanExpr s || scanBody [c1; c2]
                                           | WhileStatement(s, c)  -> scanExpr s || scanBody [c]
                                           | ExpressionStatement (Expression s)
                                           | ReturnStatement (Some s) -> scanExpr s
                                           | _ -> false) l
                              |> List.length) > 0
        and scanExpr = function
                       | UnaryExpression(_, e)
                       | ScalarAssignmentExpression(_, e)
                       | ArrayIdentifierExpression(_, e)
                       | ScalarAssignmentOperatorExpression(_, _, e) -> scanExpr e
                       | ArrayAssignmentExpression(_, e1, e2)
                       | ArrayAssignmentOperatorExpression(_, e1, _, e2)
                       | BinaryExpression(e1, _, e2) -> scanExpr e1 || scanExpr e2
                       | TernaryExpression(e1, e2, e3) -> scanExpr e1 || scanExpr e2 || scanExpr e3
                       | FunctionCallExpression _ -> true
                       | _ -> false
        snd >> scanBody

    let rec scanDeclaration =
        function
        | GlobalVariableDeclaration _ -> ()
        | FunctionDeclaration (t, i, p, b, l) ->
            if self.ContainsKey i then
                raise <| FunctionAlreadyDefined i
            else
                if l then
                    if containscall b then
                        raise <| CannotBeInlined i
                    elif i = EntryPointName then
                        raise <| MainCannotBeInlined EntryPointName
                        
                self.Add(i, {
                                IsInlined = l
                                ReturnType = t
                                ParameterTypes = List.map DeclarationType p
                            })

    do
        List.iter (fun f -> self.Add((fst f).ToString(), snd f)) BuildInFunctions
        List.iter scanDeclaration program

    member x.Program = program

type SymbolTable(program) as self =
    inherit Dictionary<IdentifierRef, VariableDeclaration>(HashIdentity.Reference)

    let whileStatementStack = Stack<WhileStatement> ()
    let symbolScopeStack = SymbolScopeStack ()

    let rec scanDeclaration = function
                              | GlobalVariableDeclaration x -> symbolScopeStack.AddDeclaration x
                              | FunctionDeclaration x -> scanFunctionDeclaration x

    and scanFunctionDeclaration (functionReturnType, _, parameters, compoundStatement, _) =
        let rec scanCompoundStatement (localDeclarations, statements) =
            symbolScopeStack.Push()
            List.iter symbolScopeStack.AddDeclaration localDeclarations
            List.iter scanStatement statements
            symbolScopeStack.Pop()
        and scanStatement = function
                            | ExpressionStatement Nop -> ()
                            | ExpressionStatement (Expression e) -> scanExpression e
                            | CompoundStatement x -> scanCompoundStatement x
                            | IfStatement (e, s1, Some s2) ->
                                scanExpression e
                                scanStatement s1
                                scanStatement s2
                            | IfStatement (e, s, None) ->
                                scanExpression e
                                scanStatement s
                            | WhileStatement (e, s) ->
                                whileStatementStack.Push (e, s)
                                scanExpression e
                                scanStatement s
                                whileStatementStack.Pop() |> ignore
                            | ReturnStatement (Some e) -> scanExpression e
                            | ReturnStatement None ->
                                if functionReturnType <> Void then
                                    raise <| CannotConvertType !/Void !/functionReturnType
                            | ContinueStatement
                            | BreakStatement ->
                                if whileStatementStack.Count = 0 then
                                    raise <| NoEnclosingLoop()
                            | HaltStatement
                            | AbkStatement
                            | InlineAssemblyStatement _ -> ()
        and addIdentifierMapping i =
            if self.ContainsKey i then
                raise <| VariableAlreadyDefined i.Identifier
            else
                self.Add(i, symbolScopeStack.CurrentScope.FindDeclaration i)
        and scanExpression = function
                             | ScalarAssignmentExpression(i, e)
                             | ScalarAssignmentOperatorExpression(i, _, e) ->
                                 addIdentifierMapping i
                                 scanExpression e
                             | ArrayAssignmentExpression(_, e1, e2)
                             | ArrayAssignmentOperatorExpression(_, e1, _, e2)
                             | BinaryExpression(e1, _, e2) ->
                                 scanExpression e1
                                 scanExpression e2
                             | TernaryExpression(e1, e2, e3) ->
                                 scanExpression e1
                                 scanExpression e2
                                 scanExpression e3
                             | ArrayIdentifierExpression(_, e)
                             | UnaryExpression(_, e) ->
                                 scanExpression e
                             | IdentifierExpression i -> addIdentifierMapping i
                             | FunctionCallExpression(_, args) -> List.iter scanExpression args
                             | ArraySizeExpression _
                             | LiteralExpression _
                             | UUIDExpression -> ()
        symbolScopeStack.Push()
        List.iter symbolScopeStack.AddDeclaration parameters
        scanCompoundStatement compoundStatement
        symbolScopeStack.Pop()

    do
        // add symbols 'io' and 'mem' ?
        List.iter scanDeclaration program
    
    member x.Program = program

    member x.GetIdentifierTypeSpec id = DeclarationType self.[id]
    
type ExpressionTypeDictionary(program, functionTable : FunctionTable, symbolTable : SymbolTable) as self =
    inherit Dictionary<Expression, VariableType>(HashIdentity.Reference)

    let rec scanDeclaration = function
                              | FunctionDeclaration x -> scanFunctionDeclaration x
                              | _ -> ()
    and scanFunctionDeclaration (functionReturnType, _, _, compoundStatement, _) =
        let rec scanCompoundStatement (_, statements) = List.iter scanStatement statements
        and scanStatement =
            function
            | ExpressionStatement es -> match es with
                                        | Expression e -> scanExpression e |> ignore
                                        | Nop -> ()
            | CompoundStatement x -> scanCompoundStatement x
            | IfStatement(e, s1, Some s2) ->
                let te = scanExpression e
                match te with
                | Sbool _ -> scanStatement s1
                             scanStatement s2
                | _ -> raise <| CannotConvertType te !/Bool
            | IfStatement(e, s, None) ->
                let te = scanExpression e
                match te with
                | Sbool _ -> scanStatement s
                | _ -> raise <| CannotConvertType te !/Bool
            | WhileStatement(e, s) ->
                let te = scanExpression e
                match te with
                | Sbool _ -> scanStatement s
                | _ -> raise <| CannotConvertType te !/Bool
            | ReturnStatement(Some e) ->
                let te = scanExpression e
                if te <> !/functionReturnType then
                    raise <| CannotConvertType te !/functionReturnType
            | _ -> ()
        and scanExpression expr =
            let checkArrayIndexType e =
                let ti = scanExpression e
                if ti <> !/Int then
                    raise <| CannotConvertType ti !/Int
            let checkScalarAssignment ti e =
                let te = scanExpression e
                if te <> ti then
                    raise <| CannotConvertType te ti
                ti
            let texpr =
                match expr with
                | ScalarAssignmentExpression(i, e) ->
                    checkScalarAssignment (symbolTable.GetIdentifierTypeSpec i) e
                | ScalarAssignmentOperatorExpression(i, op, e) ->
                    checkScalarAssignment (symbolTable.GetIdentifierTypeSpec i) (BinaryExpression(IdentifierExpression i, op, e))
                | ArrayAssignmentExpression(_, e1, e2) ->
                    checkArrayIndexType e1
                    let t2 = scanExpression e2
                    if t2.IsArray then
                        raise <| CannotConvertType t2 !/Int
                    elif t2.Type <> Int then
                        raise <| CannotConvertType t2 !/Int
                    else
                        !/Int
                | ArrayAssignmentOperatorExpression(i, e1, op, e2) ->
                    checkArrayIndexType e1
                    let t2 = scanExpression e2
                    if t2.IsArray then
                        raise <| CannotConvertType t2 !/Int
                    elif t2.Type <> Int then
                        raise <| CannotConvertType t2 !/Int
                    else
                        checkScalarAssignment !/Int (BinaryExpression(ArrayIdentifierExpression(i, e2), op, e2))
                | TernaryExpression(e1, e2, e3) ->
                    let t1 = scanExpression e1
                    let t2 = scanExpression e2
                    let t3 = scanExpression e3
                    match t1, t2, t3 with
                    | Sbool _, _, _ when t2 = t3 -> t2
                    | _ -> raise <| TernopCannotBeApplied t1 t2 t3
                | BinaryExpression(e1, op, e2) ->
                    let t1 = scanExpression e1
                    let t2 = scanExpression e2
                    match op with
                    | Or | And | Xor ->
                        match t1, t2 with
                        | Scalar a, Scalar b when a = b && a <> Void -> !/a
                        | _ -> raise <| BinopCannotBeApplied op t1 t2
                    | Equal | NotEqual ->
                        match t1, t2 with
                        | Scalar a, Scalar b when a = b && a <> Void -> !/Bool
                        | _ -> raise <| BinopCannotBeApplied op t1 t2
                    | LessEqual | Less | GreaterEqual | Greater ->
                        match t1, t2 with
                        | Sint _, Sint _ -> !/Bool
                        | _ -> raise <| BinopCannotBeApplied op t1 t2
                    | Add | Subtract | Multiply | Divide | Modulus | Power | Rol | Ror | Shr | Shl ->
                        match t1, t2 with
                        | Sint _, Sint _ -> !/Int
                        | _ -> raise <| BinopCannotBeApplied op t1 t2
                | UnaryExpression(op, e) ->
                     let te = scanExpression e
                     match op, te with
                     | LogicalNegate _, Sbool _
                     | Negate _, Sint _
                     | Incr _, Sint _
                     | Decr _, Sint _
                     | Identity _, Sint _
                     | Not _, _ -> te
                     | IntCast _, _ -> !/Int
                     | BoolCast _, _ -> !/Bool
                     | _ -> raise <| UnopCannotBeApplied op te
                | IdentifierExpression i -> symbolTable.GetIdentifierTypeSpec i
                | ArrayIdentifierExpression(_, e) ->
                    checkArrayIndexType e
                    !/Int
                | FunctionCallExpression(i, a) ->
                    if not (functionTable.ContainsKey i) then
                        raise <| NameDoesNotExist i
                    let calledFunction = functionTable.[i]
                    let parameterTypes = calledFunction.ParameterTypes
                    if List.length a <> List.length parameterTypes then
                        raise <| WrongNumberOfArguments i (List.length parameterTypes) (List.length a)
                    let argumentTypes = List.map scanExpression a
                    let checkTypesMatch index l r =
                        if l <> r then
                            raise <| InvalidArguments i (index + 1) (l.ToString()) (r.ToString())
                    List.iteri2 checkTypesMatch argumentTypes parameterTypes
                    !/calledFunction.ReturnType
                | ArraySizeExpression _
                | UUIDExpression -> !/Int
                | LiteralExpression l -> match l with
                                         | BoolLiteral _ -> !/Bool
                                         | IntLiteral _ -> !/Int
            self.Add(expr, texpr)
            texpr
        scanCompoundStatement compoundStatement
    do
        List.iter scanDeclaration program

    member x.Program = program

    member x.Functions = functionTable

    member x.Symbols = symbolTable
        
type SemanticAnalysisResult =
    {
        SymbolTable : SymbolTable;
        ExpressionTypes : ExpressionTypeDictionary;
    }

let Analyze program =
    let symbolTable = SymbolTable program
    let functionTable = FunctionTable program
    if not (functionTable.ContainsKey EntryPointName) then
        raise <| MissingEntryPoint EntryPointName
    else
        let expressionTypes = ExpressionTypeDictionary(program, functionTable, symbolTable)
        {
            SymbolTable = symbolTable;
            ExpressionTypes = expressionTypes;
        }
