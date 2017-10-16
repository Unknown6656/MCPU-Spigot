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
    | Ldmem of int
    | Ldio of int
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
    | Stmem of int * int
    | Stio of int
    | Stiodir of int * int
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
    let rec procbinexpr = function
                          | (l, SyntaxTree.Or, r) ->
                                let leftfalselabel = mklabel()
                                let endlabel = mklabel()
                                List.concat [
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
                                List.concat [
                                                procexpr l
                                                [Brtrue lefttruelabel]
                                                [Ldc 0]
                                                [Br endlabel]
                                                [Label lefttruelabel]
                                                procexpr r
                                                [Label endlabel]
                                            ]
                            | (l, op, r) -> List.concat [
                                                            procexpr l
                                                            procexpr r
                                                            [procbinop op]
                                                        ]
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
        | GlobalScope v -> [Ldglob v]
        | ArgumentScope i -> [Ldarg i]
        | LocalScope i -> [Ldloc i]
