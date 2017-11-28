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
            let mutable firstregloc = true
            let mutable loccnt = f.Parameters.Length
            let body = List.collect (fun ins -> match ins with
                                                | Call (f, n) ->
                                                    let target = fdic.[f]

                                                    if (snd target).IsInlined then
                                                        let body = (snd target).Body
                                                                    |> List.takeWhile (function
                                                                                        | Ret -> false
                                                                                        | _ -> true)
                                                        [
                                                            Comment(ASMCommentLevel.CommentStatementsAndExpressions, sprintf "begin of inlining '%s' ..." (snd target).Signature)
                                                            // TODO
                                                            
                                                        ]
                                                        @ body
                                                        @ [Comment(ASMCommentLevel.CommentStatementsAndExpressions, sprintf "end of inlining '%s'."  (snd target).Signature)]
                                                    else
                                                        [Call (sprintf "%d" (fst target), n)]
                                                 | Regloc n ->
                                                    if n > loccnt then
                                                        loccnt <- loccnt + n

                                                    if firstregloc then
                                                        firstregloc <- false
                                                        []
                                                    else
                                                        List.collect (fun x -> [Ldc 0; Starg x]) [0s..int16 n]
                                                 | _ -> [ins]) f.Body
            
            [
                Comment(ASMCommentLevel.CommentStatementsAndExpressions, sprintf "function '%s'" f.Signature)
                Label <| fst fdic.[f.Name]
                Regloc loccnt
            ] @ body @ [
                Ret
                Comment(ASMCommentLevel.CommentStatementsAndExpressions, sprintf "end of function '%s'" f.Signature)
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
    
    member x.Generate level = 
        x.Merge()
        |> List.collect (fun l -> match l with
                                  | Comment _ -> [l]
                                  | _ -> [
                                             Comment(ASMCommentLevel.CommentInstructions, l.DescriptionString)
                                             l
                                         ])
        |> List.filter (fun x -> match level, x with
                                 | x, Comment(y, _) when x = y -> true
                                 | ASMCommentLevel.CommentAll, _
                                 | ASMCommentLevel.CommentStatementsAndExpressions, Comment(ASMCommentLevel.CommentExpressions, _) -> true
                                 | _, Comment _ -> false
                                 | _ -> true)
        |> List.filter (fun x -> match x with
                                 | Comment(_, s) -> if s = null then false else s.Trim().Length > 0
                                 | _ -> true)
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
                    | Max -> "max"
                    | Min -> "min"
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
                    | UUID -> "uuid"
                    | Xor -> "xor"
                    | Comment(l, s) -> sprintf "; %s: %s" (match l with
                                                           | ASMCommentLevel.CommentExpressions -> "EXP"
                                                           | ASMCommentLevel.CommentStatementsAndExpressions -> "STM"
                                                           | ASMCommentLevel.CommentInstructions -> "INS"
                                                           | _ -> "ALL") s)
        |> List.toArray
        