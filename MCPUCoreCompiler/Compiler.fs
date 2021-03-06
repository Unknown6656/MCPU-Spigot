﻿module MCPUCompiler.Core.Compiler

open System.Collections.Generic
open MCPUCompiler.Core.Parser
open SyntaxTree


type ASMLabel = uint64

type ASMVariable =
    {
        Name : string
    }

type ASMCommentLevel =
    | NoComments = 0
    | CommentExpressions = 1
    | CommentStatementsAndExpressions = 2
    | CommentInstructions = 3
    | CommentAll = 4

type ASMInstruction =
    | Comment of ASMCommentLevel * string
    | Abk
    | Abs
    | Add
    | And
    | Br of ASMLabel
    | Brtrue of ASMLabel
    | Brfalse of ASMLabel
    | Call of string * int
    | Div
    | Dup
    | Eq
    | Exp
    | Geq
    | Gt
    | Halt
    | Label of ASMLabel
    | Ldglob of ASMVariable
    | Ldarg of int16
    | Ldloc of int16
    | Ldmem
    | Ldio
    | Ldmemsz
    | Ldiosz
    | Ldc of int
    | Lt
    | Leq
    | Log
    | Loge
    | Log2
    | Log10
    | Max
    | Min
    | Mod
    | Mul
    | Neg
    | Neq
    | Not
    | Or
    | Pop
    | Pow
    | Raw of string
    | Regloc of int
    | Regglob of int
    | Ret
    | Rol
    | Ror
    | Shl
    | Shr
    | Sign
    | Starg of int16
    | Stloc of int16
    | Stglob of ASMVariable
    | Stmem
    | Stio
    | Stiodir
    | Sub
    | Swap
    | Syscall of int
    | UUID
    | Xor
    member x.DescriptionString =
        let (!@) = sprintf
        function
        | Abs -> "Takes the top-most stack value and returns its absolute interger part."
        | Add -> "Calculate the addition of the two top-most stack values and push the result back onto the stack."
        | And -> "Calculate the binary AND-combination of the two top-most stack values and push the result back onto the stack."
        | Br l -> !@"Jump to the label '%016x'." l
        | Brtrue l -> !@"Jump to the label '%016x' if the top-most stack value is not equal to zero." l
        | Brfalse l -> !@"Jump to the label '%016x' if the top-most stack value is equal to zero." l
        | Call(s, i) -> !@"Call the function '%s' and pass the %d top-most stack values as call arguments." s i
        | Div -> "Calculate the division of the two top-most stack values and push the result back onto the stack."
        | Dup -> "Duplicate the top-most stack value."
        | Eq -> "Compare whether the two top-most stack values are equal and push the comparison result back onto the stack."
        | Exp -> "Calculate top-most value raised to the power of e and push the result back onto the stack."
        | Geq -> "Compare whether the second top-most stack value is greater or equal to the top-most one and push the comparison result back onto the stack."
        | Gt -> "Compare whether the second top-most stack value is greater than the top-most one and push the comparison result back onto the stack."
        | Halt -> "Halt the processor."
        | Ldglob v -> !@"Load the global variable '%s' onto the stack." v.Name
        | Ldarg i -> !@"Load the argument no. %d onto the stack." i
        | Ldloc i -> !@"Load the local value no. %d onto the stack." i
        | Ldmem -> "Load the value stored at the memory address indicated by the top-most stack value onto the stack - thereby consuming the memory address."
        | Ldio -> "Load the value from the I/O-port indicated by the top-most stack value onto the stack - thereby consuming the I/O-port number."
        | Ldmemsz -> "Load the memory size onto the stack."
        | Ldiosz -> "Load the I/O-port count onto the stack."
        | Ldc i -> !@"Load the constant %d (%08x) onto the stack." i i
        | Lt -> "Compare whether the second top-most stack value is less than the top-most one and push the comparison result back onto the stack."
        | Leq -> "Compare whether the second top-most stack value is less than or equal to the top-most one and push the comparison result back onto the stack."
        | Log -> "Calculate the integer logarithm of the top-most stack value using the base stored in the second top-most stack value and push the result back onto the stack."
        | Loge -> "Calculate the integer logarithm of the top-most stack value using the base e and push the result back onto the stack."
        | Log2 -> "Calculate the integer logarithm of the top-most stack value using the base 2 and push the result back onto the stack."
        | Log10 -> "Calculate the integer logarithm of the top-most stack value using the base 10 and push the result back onto the stack."
        | Max -> "Determine the greater of the two top-most stack values and push the result back onto the stack."
        | Min -> "Determine the smaller of the two top-most stack values and push the result back onto the stack."
        | Mod -> "Calculate the integer division remainder of the two top-most stack values and push the result back onto the stack."
        | Mul -> "Calculate the multiplication of the two top-most stack values and push the result back onto the stack."
        | Neg -> "Negate the top-most stack value by multiplying it with -1 and push the result back onto the stack."
        | Neq -> "Compare whether the two top-most stack values are not equal and push the comparison result back onto the stack."
        | Not -> "Invert the top-most stack value by flipping its bits and push the result back onto the stack."
        | Or -> "Calculate the binary OR-combination of the two top-most stack values and push the result back onto the stack."
        | Pop -> "Pop the top-most stack value"
        | Pow -> "Calculate second top-most value raised to the power of the top-most one and push the result back onto the stack."
        | Regloc i -> !@"Register the size %d as number of local variables in the current stack frame." i
        | Regglob i -> !@"Register the size %d as number of global variables in the current application." i
        | Ret -> "Return the execution to the previous stack frame and pass the top-most stack value back as function result (if existent)."
        | Rol -> "Calculate the binary RotateLeft-combination of the two top-most stack values and push the result back onto the stack."
        | Ror -> "Calculate the binary RotateRight-combination of the two top-most stack values and push the result back onto the stack."
        | Shl -> "Calculate the binary ShiftLeft-combination of the two top-most stack values and push the result back onto the stack."
        | Shr -> "Calculate the binary ShiftRight-combination of the two top-most stack values and push the result back onto the stack."
        | Sign -> "Determine the signum of the top-most value and push the result back onto the stack."
        | Starg i -> !@"Pop the top-most value from the stack and store it into the argument no. %d." i
        | Stloc i -> !@"Pop the top-most value from the stack and store it into the local variable no. %d." i
        | Stglob v -> !@"Pop the top-most value from the stack and store it into the global variable '%s'." v.Name
        | Stmem -> "Store the top-most stack value into the memory address indicated by the second top-most stack value."
        | Stio -> "Write the top-most stack value to the I/O-port indicated by the second top-most stack value."
        | Stiodir -> "Sets the I/O-port indicated by the second top-most stack value to the direction indicated by the top-most one."
        | Sub -> "Calculate the subtraction of the two top-most stack values and push the result back onto the stack."
        | Swap -> "Swap the two top-most stack values."
        | Syscall i -> !@"Call the system-call no. %d." i
        | UUID -> "Load the processor's int32-UUID onto the stack."
        | Xor -> "Calculate the binary XOR-combination of the two top-most stack values and push the result back onto the stack."
        | _ -> null
       <| x


type ASMMethod =
    {
        Name : string
        IsInlined : bool
        ReturnType : TypeSpec
        Parameters : ASMVariable list
        Locals : ASMVariable list
        Body : ASMInstruction list
    }
    override x.ToString() =
        sprintf "(\n\tName: %s\n\tInlined: %A\n\tReturnType: %A\n\tParameters: %A\n\tLocals: %A\n\tBody: (\n\t%s)\n)"
            x.Name x.IsInlined x.ReturnType x.Parameters x.Locals (String.concat "" [for i in x.Body -> "\t" + i.ToString() + "\n\t"])
    member x.Signature =
        let p = String.concat ", " (List.map (fun (f : ASMVariable) -> "data " + f.Name) x.Parameters)
        let p = if p.Length = 0 then "void" else p
        sprintf "%s : %s -> %s" x.Name p <| x.ReturnType.ToString().ToLower()


type ASMProgram =
    {
        Globals : ASMVariable list
        Methods : ASMMethod list
    }

type ASMVariableScope =
    | GlobalScope of ASMVariable
    | ArgumentScope of int16
    | LocalScope of int16

type VariableMappingDictionary = Dictionary<VariableDeclaration, ASMVariableScope>

let variable n = { ASMVariable.Name = n }
let CreateASMVariable (d : VariableDeclaration) = variable (snd d)

type ASMMethodBuilder(sares : SemanticAnalysisResult, mapping : VariableMappingDictionary) =
    let mutable argindex = 0s
    let mutable locindex = 0s
    let mutable lbindex = 0UL
    let arrassgnloc = Dictionary<Expression, int16>()
    let endlabelst = Stack<ASMLabel>()
    let condlabelst = Stack<ASMLabel>()
    
    let lookupScope i =
        let decl = sares.SymbolTable.[i]
        mapping.[decl]

    let mklabel () =
        let res = lbindex
        lbindex <- lbindex + 1UL
        res

    let rec procbinexpr expr =
        [Comment(ASMCommentLevel.CommentExpressions, "binary expression '" + expr.ToString().Replace('\n', ' ') + "'")]
        @ ((function
            | (l, SyntaxTree.Or, r) ->
                let leftfalselabel = mklabel()
                let endlabel = mklabel()
                [
                    procexpr l
                    [Brfalse leftfalselabel]
                    [Ldc 1]
                    [Br endlabel]
                    [Label leftfalselabel]
                    procexpr r
                    [Label endlabel]
                ]
            | (l, SyntaxTree.And, r) ->
                let lefttruelabel = mklabel()
                let endlabel = mklabel()
                [
                    procexpr l
                    [Brtrue lefttruelabel]
                    [Ldc 0]
                    [Br endlabel]
                    [Label lefttruelabel]
                    procexpr r
                    [Label endlabel]
                ]
            | (l, op, r) ->
                [
                    procexpr l
                    procexpr r
                    [procbinop op]
                ]) >> List.concat) expr

    and procunop x = match x with
                     | LogicalNegate ->
                        [
                            Ldc 0
                            Eq
                        ]
                     | Negate -> [Neg]
                     | SyntaxTree.Not -> [Not]
                     | Incr ->
                        [
                            Ldc 1
                            Add
                        ]
                     | Decr ->
                        [
                            Ldc 1
                            Sub
                        ]
                     | Identity
                     | IntCast -> []
                     | BoolCast -> 
                        [
                            Ldc 0
                            Neq
                        ]
                     | _ -> raise <| GeneratorError x

    and procbinop x = match x with
                      | SyntaxTree.Add -> Add
                      | Equal -> Eq
                      | NotEqual -> Neq
                      | LessEqual -> Leq
                      | Less -> Lt
                      | GreaterEqual -> Geq
                      | Greater -> Gt
                      | Subtract -> Sub
                      | Multiply -> Mul
                      | Divide -> Div
                      | Modulus -> Mod
                      | Power -> Pow
                      | SyntaxTree.Xor -> Xor
                      | SyntaxTree.Shr -> Shr
                      | SyntaxTree.Shl -> Shl
                      | SyntaxTree.Ror -> Ror
                      | SyntaxTree.Rol -> Rol
                      | _ -> raise <| GeneratorError x

    and procidentifierload i =
        match lookupScope i with
        | GlobalScope v -> Ldglob v
        | ArgumentScope i -> Ldarg i
        | LocalScope i -> Ldloc i

    and procidentifierstore i =
        match lookupScope i with
        | GlobalScope v -> Stglob v
        | ArgumentScope i -> Starg i
        | LocalScope i -> Stloc i

    and procexpr expr =
        [Comment(ASMCommentLevel.CommentExpressions, "expression '"  + expr.ToString().Replace('\n', ' ') + "'")]
        @ List.concat (match expr with
                       | ScalarAssignmentExpression(i, e) ->
                           [
                               procexpr e
                               [Dup]
                               [procidentifierstore i]
                           ]
                       | ScalarAssignmentOperatorExpression(i, o, e) ->
                           [
                               [procidentifierload i]
                               procexpr e
                               [procbinop o]
                               [Dup]
                               [procidentifierstore i]
                           ]
                       | TernaryExpression(c, x, y) ->
                           let endlabel = mklabel()
                           let falselabel = mklabel()
                           [
                               procexpr c
                               [Brfalse falselabel]
                               procexpr x
                               [Br endlabel]
                               [Label falselabel]
                               procexpr y
                               [Label endlabel]
                           ]
                       | BinaryExpression(x, o, y) -> [procbinexpr(x, o, y)]
                       | UnaryExpression(o, x) ->
                           [
                               procexpr x
                               procunop o
                           ]
                       | IdentifierExpression i -> [[procidentifierload i]]
                       | ArrayIdentifierExpression(i, n) ->
                           [
                               procexpr n
                               [( if i = IO then Ldio else Ldmem )]
                           ]
                       | ArrayAssignmentExpression(i, n, e) ->
                           [
                               procexpr e
                               [Dup]
                               procexpr n
                               [Swap]
                               [( if i = IO then Stio else Stmem )]
                           ]
                       | ArrayAssignmentOperatorExpression(i, n, o, e) ->
                           [
                               procexpr n
                               [( if i = IO then Ldio else Ldmem )]
                               procexpr e
                               [procbinop o]
                               procexpr n
                               [Swap]
                               [( if i = IO then Stio else Stmem )]
                           ]
                       | LiteralExpression l ->
                           [[Ldc (match l with
                                  | IntLiteral i -> i
                                  | BoolLiteral b -> if b then 1 else 0)]]
                       | ArraySizeExpression i ->
                           [[( if i = IO then Ldmemsz else Ldiosz )]]
                       | FunctionCallExpression(i, p) ->
                           [
                               List.collect procexpr p
                               [Call(i, p.Length)]
                           ]
                       | UUIDExpression -> [[UUID]]
        )
    
    and procstatm stm =
        [Comment(ASMCommentLevel.CommentStatementsAndExpressions, "statement '"  + stm.ToString().Replace('\n', ' ') + "'")]
        @ List.concat (match stm with
                       | ExpressionStatement e ->
                           match e with
                           | Expression x ->
                               let isvoid = sares.ExpressionTypes.[x].Type = Void
                               [
                                   procexpr x
                                   (if isvoid then [] else [Pop])
                               ]
                           | Nop -> []
                       | CompoundStatement(_, s) -> List.map procstatm s
                       | InlineAssemblyStatement r -> [[Raw r]]
                       | IfStatement(c, s1, Some s2) ->
                           let thenlabel = mklabel()
                           let endlabel = mklabel()
                           [
                               procexpr c
                               [Brtrue thenlabel]
                               procstatm s2
                               [Br endlabel]
                               [Label thenlabel]
                               procstatm s1
                               [Label endlabel]
                           ]
                       | IfStatement(c, s, None) ->
                           let endlabel = mklabel()
                           [
                               procexpr c
                               [Brfalse endlabel]
                               procstatm s
                               [Label endlabel]
                           ]
                       | WhileStatement(c, s) ->
                           let startlabel = mklabel()
                           let condlabel = mklabel()
                           let endlabel = mklabel()
                           endlabelst.Push endlabel
                           condlabelst.Push condlabel
                           let res = [
                                         [Br condlabel]
                                         [Label startlabel]
                                         procstatm s
                                         [Label condlabel]
                                         procexpr c
                                         [Brtrue startlabel]
                                         [Label endlabel]
                                     ]
                           ignore <| endlabelst.Pop()
                           ignore <| condlabelst.Pop()
                           res
                       | ContinueStatement -> [[Br (condlabelst.Peek())]]
                       | BreakStatement -> [[Br (endlabelst.Peek())]]
                       | ReturnStatement None -> [[Ret]]
                       | ReturnStatement (Some x) ->
                           [
                               procexpr x
                               [Ret]
                           ]
                       | AbkStatement -> [[Abk]]
                       | HaltStatement -> [[Halt]]
        )

    let procvardecl (mi : byref<_>) f d =
        let var = CreateASMVariable d
        let scope = f mi
        if mapping.ContainsKey d then
            raise <| VariableAlreadyDefined (snd d)
        else
            mapping.Add(d, scope)
            mi <- mi + 1s
            var

    let proclocdecl decl = procvardecl &locindex LocalScope decl

    let procparam decl = procvardecl &argindex ArgumentScope decl
    
    let rec collectlocdecl stm =
        let constrvar expr =
            let var = { ASMVariable.Name = sprintf "___tmp<>%016x" locindex }
            arrassgnloc.Add(expr, locindex)
            locindex <- locindex + 1s
            var

        let rec fromexpr = function
                           | ScalarAssignmentExpression(_, e) -> fromexpr e
                           | ArrayAssignmentOperatorExpression(_, n, _, e) as ae -> (fromexpr n)@[constrvar ae]@(fromexpr e)
                           | ArrayAssignmentExpression(_, n, e) as ae -> (fromexpr n)@[constrvar ae]@(fromexpr e)
                           | BinaryExpression(l, _, r) -> (fromexpr l)@(fromexpr r)
                           | TernaryExpression(c, t, f) -> List.collect fromexpr [c;t;f]
                           | ArrayIdentifierExpression(_, e)
                           | UnaryExpression(_, e) -> fromexpr e
                           | FunctionCallExpression(_, a) -> List.collect fromexpr a
                           | _ -> []

        let fromstat stm =
            match stm with
            | ExpressionStatement (Expression e) -> [fromexpr e]
            | ExpressionStatement (Nop) -> []
            | CompoundStatement(loc, stms) -> 
                [
                    List.map proclocdecl loc
                    List.collect collectlocdecl stms
                ]
            | IfStatement(c, s1, Some s2) ->
                [
                    fromexpr c
                    collectlocdecl s1
                    collectlocdecl s2
                ]
            | WhileStatement(c, s)
            | IfStatement(c, s, None) ->
                [
                    fromexpr c
                    collectlocdecl s
                ]
            | ReturnStatement(Some r) -> [fromexpr r]
            | _ -> []
            |> List.concat
        fromstat stm

    member __.BuildMethod (rettype, name, para, (locdec, stms), inlined) =
        let locals = List.concat [
                                    List.map proclocdecl locdec
                                    List.collect collectlocdecl stms
                                 ]
        {
            ASMMethod.Name = name
            ReturnType = rettype
            Parameters = List.map procparam para
            Locals = locals
            IsInlined = inlined
            Body = Regloc locals.Length :: List.collect procstatm stms
        }

type ASMBuilder (sares : SemanticAnalysisResult) =
    let varmap = VariableMappingDictionary HashIdentity.Structural

    let procstatvardecl d =
        let v = CreateASMVariable d
        varmap.Add(d, GlobalScope v)
        v

    let procfuncdecl =
        let asmbuilder = ASMMethodBuilder(sares, varmap)
        asmbuilder.BuildMethod

    member __.VariableMap = varmap

    member __.BuildClass (prog : SyntaxTree.Program) =
        let builtin = 
            let nv s = { ASMVariable.Name = s }
            [
                Func__abs, Int, 1, 0, [
                    Ldarg 0s
                    Abs
                ]
                Func__min, Int, 2, 0, [
                    Ldarg 0s
                    Ldarg 1s
                    Min
                ]
                Func__max, Int, 2, 0, [
                    Ldarg 0s
                    Ldarg 1s
                    Max
                ]
                Func__sign, Int, 1, 0, [
                    Ldarg 0s
                    Sign
                ]
                Func__pow, Int, 2, 0, [
                    Ldarg 0s
                    Ldarg 1s
                    Pow
                ]
                Func__log2, Int, 1, 0, [
                    Ldarg 0s
                    Log2
                ]
                Func__log10, Int, 1, 0, [
                    Ldarg 0s
                    Log10
                ]
                Func__exp, Int, 1, 0, [
                    Ldarg 0s
                    Exp
                ]
                Func__iodir, Void, 2, 0, [
                    Ldarg 0s
                    Ldarg 1s
                    Stiodir
                ]
                Func__printi, Void, 1, 0, [
                    Ldarg 0s
                    Syscall 1
                    Pop
                ]
            ]
            |> List.map (fun (f, t, p, l, b) -> {
                                                    ASMMethod.Name = f.ToString()
                                                    IsInlined = true
                                                    ReturnType = t
                                                    Parameters = if p < 1 then []
                                                                 else List.map (fun x -> nv <| "arg" + x.ToString()) [1..p]
                                                    Locals = if l < 1 then []
                                                             else List.map (fun x -> nv <| "loc" + x.ToString()) [1..l]
                                                    Body = b //  @ [Ret]
                                                })
        {
            Globals = prog
                      |> List.choose (fun x -> match x with
                                               | GlobalVariableDeclaration x -> Some x
                                               | _ -> None) 
                      |> List.map procstatvardecl
            Methods =
                let methods = prog
                              |> List.choose (fun x -> match x with
                                                       | FunctionDeclaration a -> Some a
                                                       | _ -> None)
                              |> List.map procfuncdecl
                let containscall target =
                    List.exists (function
                                 | Call (n, _) -> n = target.Name
                                 | _ -> false)
                (builtin
                 |> List.filter (fun b -> List.exists (fun f -> containscall b f.Body) methods)
                ) @ methods
        }
