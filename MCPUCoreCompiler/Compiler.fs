module MCPUCompiler.Core.Compiler

open System.Collections.Generic
open MCPUCompiler.Core.Parser
open SyntaxTree


type ASMLabel = int

type ASMVariable =
    {
        Name : string
    }

type ASMInstruction =
    | Abk
    | Abs
    | Add
    | And
    | Br of ASMLabel
    | Brtrue of ASMLabel
    | Brfalse of ASMLabel
    | Call of string
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
    | Ldc of int
    | Lt
    | Leq
    | Log
    | Loge
    | Log2
    | Log10
    | Mod
    | Mul
    | Neg
    | Neq
    | Not
    | Or
    | Pow
    | Regloc of int
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
    | Xor

type ASMMethod =
    {
        Name : string
        ReturnType : TypeSpec
        Parameters : ASMVariable list
        Locals : ASMVariable list
        Body : ASMInstruction list
    }

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
    let mutable lbindex = 0
    let arrassgnloc = Dictionary<Expression, int16>()
    let endlabel = Stack<ASMLabel>()
    
    let lookupScope i =
        let decl = sares.SymbolTable.[i]
        mapping.[decl]
    let mklabel () =
        let res = lbindex
        lbindex <- lbindex + 1
        res
    let rec procbinexpr = (function
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
                                ]
                            ) >> List.concat
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
        match expr with
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
        | ArrayAssignmentExpression (i, n, e) ->
            [
                procexpr e
                [Dup]
                procexpr n
                [Swap]
                [( if i = IO then Stio else Stmem )]
            ]
        |> List.concat