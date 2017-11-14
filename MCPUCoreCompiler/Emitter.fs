namespace MCPUCompiler.Core

open Compiler
open Parser
open Lexer
open System

type Emitter (prog : ASMProgram) =
    let mutable lbindex = 0xffffffffffffffffUL
    let nextlabel() =
        let tmp = lbindex
        lbindex <- lbindex - 1UL
        tmp
    let lbstr : uint64 -> string = sprintf "label__%016xh"
        
    member x.Merge() =
        let glob = prog.Globals
        let func = prog.Methods
        let fdic = dict [for f in func -> f.Name, (nextlabel(), f)]
        let toasm (f : ASMMethod) =
            [
                Comment <| sprintf "function '%s'" f.Name
                Label <| fst fdic.[f.Name]
                Regloc f.Locals.Length
            ]
            @ List.map (fun ins -> match ins with
                                   | Call (f, n) -> Call (sprintf "%d" (fst fdic.[f]), n)
                                   | _ -> ins) f.Body
            @ [
                Ret
                Comment <| sprintf "end of function '%s'" f.Name
            ]
        [
            Regglob glob.Length
            Call((fst fdic.[EntryPointName]).ToString(), 0)
            Halt
        ] @ (
            List.filter (fun f -> f.Name <> EntryPointName) func
            @ [snd fdic.[EntryPointName]]
            |> List.map toasm
            |> List.concat
        )
    
    member x.Generate() = 
        ()
        |> x.Merge
        |> List.map (function
                    | Abk -> "abk"
                    | Abs -> "abs"
                    | Add -> "add"
                    | And -> "and"
                    | Br lb -> "br " + lbstr lb
                    | Brtrue lb -> "brtrue " + lbstr lb
                    | Brfalse lb -> "brfalse " + lbstr lb
                    | Call(lb, cnt) -> sprintf "call %s %d" (lbstr <| UInt64.Parse lb) cnt
                    | Div -> "div"
                    | Dup -> "dup"
                    | Eq -> "eq"
                    | Exp -> "exp"
                    | Geq -> "geq"
                    | Gt -> "gt"
                    | Halt -> "halt"
                    | Label lb -> lbstr lb + ":"
                    | Ldglob var -> failwith "TODO: ldglob"
                    | Ldarg ndx -> sprintf "ldarg %xh" ndx
                    | Ldloc ndx -> sprintf "ldloc %xh" ndx
                    | Ldmem -> "ldmem"
                    | Ldio -> "ldio"
                    | Ldmemsz -> "ldmemsz"
                    | Ldiosz -> "ldiosz"
                    | Ldc i -> sprintf "ldc %08xh" i
                    | Lt -> "lt"
                    | Leq -> "leq"
                    | Log -> "log"
                    | Loge -> "loge"
                    | Log2 -> "log2"
                    | Log10 -> "log10"
                    | Mod -> "mod"
                    | Mul -> "mul"
                    | Neg -> "neg"
                    | Neq -> "neq"
                    | Not -> "not"
                    | Or -> "or"
                    | Pop -> "pop"
                    | Pow -> "pow"
                    | Raw asm -> asm.Replace('\r', '\n').Replace("\n\n", "\n")
                    | Regloc num -> sprintf "regloc %xh" num
                    | Regglob num -> sprintf "regglob %xh" num
                    | Ret -> "ret"
                    | Rol -> "rol"
                    | Ror -> "ror"
                    | Shl -> "shl"
                    | Shr -> "shr"
                    | Sign -> "sign"
                    | Starg ndx -> sprintf "starg %xh" ndx
                    | Stloc ndx -> sprintf "stloc %xh" ndx
                    | Stglob var -> failwith "TODO: stglob"
                    | Stmem -> "stmem"
                    | Stio -> "stio"
                    | Stiodir -> "stiodir"
                    | Sub -> "sub"
                    | Swap -> "swap"
                    | Syscall i -> sprintf "syscall %xh" i
                    | Xor -> "xor"
                    | Comment s -> "// " + s)
        |> List.toArray
        