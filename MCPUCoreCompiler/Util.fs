module MCPUCompiler.Util

open Piglet.Parser
open Piglet.Parser.Configuration

type ProductionWrapperBase (production : IProduction<obj>) =
    member x.Production = production
    member x.SetReduceToFirst () = production.SetReduceToFirst()
    member x.SetPrecedence(precedenceGroup) = production.SetPrecedence(precedenceGroup)

type ProductionWrapper<'T> (production : IProduction<obj>) =
    inherit ProductionWrapperBase(production)
    member x.SetReduceFunction (f : (unit -> 'T)) =
        production.SetReduceFunction (fun o -> box (f ()))

type ProductionWrapper<'a,'T> (production : IProduction<obj>) =
    inherit ProductionWrapperBase(production)
    member x.SetReduceFunction (f : ('a -> 'T)) =
        production.SetReduceFunction (fun o -> box (f (unbox o.[0])))

type ProductionWrapper<'a,'b,'T> (production : IProduction<obj>) =
    inherit ProductionWrapperBase(production)
    member x.SetReduceFunction (f : ('a -> 'b -> 'T)) =
        production.SetReduceFunction (fun o -> box (f (unbox o.[0]) (unbox o.[1])))

type ProductionWrapper<'a,'b,'c,'T> (production : IProduction<obj>) =
    inherit ProductionWrapperBase(production)
    member x.SetReduceFunction (f : ('a -> 'b -> 'c -> 'T)) =
        production.SetReduceFunction (fun o -> box (f (unbox o.[0]) (unbox o.[1]) (unbox o.[2])))

type ProductionWrapper<'a,'b,'c,'d,'T> (production : IProduction<obj>) =
    inherit ProductionWrapperBase(production)
    member x.SetReduceFunction (f : ('a -> 'b -> 'c -> 'd -> 'T)) =
        production.SetReduceFunction (fun o -> box (f (unbox o.[0]) (unbox o.[1]) (unbox o.[2]) (unbox o.[3])))

type ProductionWrapper<'a,'b,'c,'d,'e,'T> (production : IProduction<obj>) =
    inherit ProductionWrapperBase(production)
    member x.SetReduceFunction (f : ('a -> 'b -> 'c -> 'd -> 'e -> 'T)) =
        production.SetReduceFunction (fun o -> box (f (unbox o.[0]) (unbox o.[1]) (unbox o.[2]) (unbox o.[3]) (unbox o.[4])))

type ProductionWrapper<'a,'b,'c,'d,'e,'f,'T> (production : IProduction<obj>) =
    inherit ProductionWrapperBase(production)
    member x.SetReduceFunction (f : ('a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'T)) =
        production.SetReduceFunction (fun o -> box (f (unbox o.[0]) (unbox o.[1]) (unbox o.[2]) (unbox o.[3]) (unbox o.[4]) (unbox o.[5])))

type ProductionWrapper<'a,'b,'c,'d,'e,'f,'g,'T> (production : IProduction<obj>) =
    inherit ProductionWrapperBase(production)
    member x.SetReduceFunction (f : ('a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g -> 'T)) =
        production.SetReduceFunction (fun o -> box (f (unbox o.[0]) (unbox o.[1]) (unbox o.[2]) (unbox o.[3]) (unbox o.[4]) (unbox o.[5]) (unbox o.[6])))

type ProductionWrapper<'a,'b,'c,'d,'e,'f,'g, 'h,'T> (production : IProduction<obj>) =
    inherit ProductionWrapperBase(production)
    member x.SetReduceFunction (f : ('a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g -> 'h -> 'T)) =
        production.SetReduceFunction (fun o -> box (f (unbox o.[0]) (unbox o.[1]) (unbox o.[2]) (unbox o.[3]) (unbox o.[4]) (unbox o.[5]) (unbox o.[6]) (unbox o.[7])))

type SymbolWrapper<'T> (symbol : ISymbol<obj>) =
    member x.Symbol = symbol

type TerminalWrapper<'T> (terminal : ITerminal<obj>) =
    inherit SymbolWrapper<'T>(terminal)

type NonTerminalWrapper<'T> (nonTerminal : INonTerminal<obj>) =
    inherit SymbolWrapper<'T>(nonTerminal)
    
    member x.AddProduction () =
        nonTerminal.AddProduction()
        |> ProductionWrapper<'T>
        
    member x.AddProduction (p : SymbolWrapper<'a>) =
        nonTerminal.AddProduction p.Symbol
        |> ProductionWrapper<'a,'T>
    
    member x.AddProduction((p1 : SymbolWrapper<'a>), (p2 : SymbolWrapper<'b>)) =
        nonTerminal.AddProduction(p1.Symbol, p2.Symbol)
        |> ProductionWrapper<'a,'b,'T>

    member x.AddProduction ((p1 : SymbolWrapper<'a>), (p2 : SymbolWrapper<'b>), (p3 : SymbolWrapper<'c>)) =
        nonTerminal.AddProduction(p1.Symbol, p2.Symbol, p3.Symbol)
        |> ProductionWrapper<'a,'b,'c,'T>

    member x.AddProduction((p1 : SymbolWrapper<'a>), (p2 : SymbolWrapper<'b>), (p3 : SymbolWrapper<'c>), (p4 : SymbolWrapper<'d>)) =
        nonTerminal.AddProduction(p1.Symbol, p2.Symbol, p3.Symbol, p4.Symbol)
        |> ProductionWrapper<'a,'b,'c,'d,'T>

    member x.AddProduction((p1 : SymbolWrapper<'a>), (p2 : SymbolWrapper<'b>), (p3 : SymbolWrapper<'c>),
                           (p4 : SymbolWrapper<'d>), (p5 : SymbolWrapper<'e>)) =
        nonTerminal.AddProduction(p1.Symbol, p2.Symbol, p3.Symbol, p4.Symbol, p5.Symbol)
        |> ProductionWrapper<'a,'b,'c,'d,'e,'T>

    member x.AddProduction((p1 : SymbolWrapper<'a>), (p2 : SymbolWrapper<'b>), (p3 : SymbolWrapper<'c>), (p4 : SymbolWrapper<'d>),
                           (p5 : SymbolWrapper<'e>), (p6 : SymbolWrapper<'f>)) =
        nonTerminal.AddProduction(p1.Symbol, p2.Symbol, p3.Symbol, p4.Symbol, p5.Symbol, p6.Symbol)
        |> ProductionWrapper<'a,'b,'c,'d,'e,'f,'T>

    member x.AddProduction((p1 : SymbolWrapper<'a>), (p2 : SymbolWrapper<'b>), (p3 : SymbolWrapper<'c>), (p4 : SymbolWrapper<'d>),
                           (p5 : SymbolWrapper<'e>), (p6 : SymbolWrapper<'f>), (p7 : SymbolWrapper<'g>)) =
        nonTerminal.AddProduction(p1.Symbol, p2.Symbol, p3.Symbol, p4.Symbol, p5.Symbol, p6.Symbol, p7.Symbol)
        |> ProductionWrapper<'a,'b,'c,'d,'e,'f,'g,'T>
        
    member x.AddProduction((p1 : SymbolWrapper<'a>), (p2 : SymbolWrapper<'b>), (p3 : SymbolWrapper<'c>), (p4 : SymbolWrapper<'d>),
                           (p5 : SymbolWrapper<'e>), (p6 : SymbolWrapper<'f>), (p7 : SymbolWrapper<'g>), (p8 : SymbolWrapper<'h>)) =
        nonTerminal.AddProduction(p1.Symbol, p2.Symbol, p3.Symbol, p4.Symbol, p5.Symbol, p6.Symbol, p7.Symbol, p8.Symbol)
        |> ProductionWrapper<'a,'b,'c,'d,'e,'f,'g,'h,'T>
