module MCPUCompiler.Core.Util

open Piglet.Parser
open Piglet.Parser.Configuration


let private (+>) (o : obj[]) n = unbox o.[n]

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
        production.SetReduceFunction (fun o -> o+>0
                                               |> f
                                               |> box)

type ProductionWrapper<'a,'b,'T> (production : IProduction<obj>) =
    inherit ProductionWrapperBase(production)
    member x.SetReduceFunction (f : ('a -> 'b -> 'T)) =
        production.SetReduceFunction (fun o -> box (f (o+>0) (o+>1)))

type ProductionWrapper<'a,'b,'c,'T> (production : IProduction<obj>) =
    inherit ProductionWrapperBase(production)
    member x.SetReduceFunction (f : ('a -> 'b -> 'c -> 'T)) =
        production.SetReduceFunction (fun o -> box (f (o+>0) (o+>1) (o+>2)))

type ProductionWrapper<'a,'b,'c,'d,'T> (production : IProduction<obj>) =
    inherit ProductionWrapperBase(production)
    member x.SetReduceFunction (f : ('a -> 'b -> 'c -> 'd -> 'T)) =
        production.SetReduceFunction (fun o -> box (f (o+>0) (o+>1) (o+>2) (o+>3)))

type ProductionWrapper<'a,'b,'c,'d,'e,'T> (production : IProduction<obj>) =
    inherit ProductionWrapperBase(production)
    member x.SetReduceFunction (f : ('a -> 'b -> 'c -> 'd -> 'e -> 'T)) =
        production.SetReduceFunction (fun o -> box (f (o+>0) (o+>1) (o+>2) (o+>3) (o+>4)))

type ProductionWrapper<'a,'b,'c,'d,'e,'f,'T> (production : IProduction<obj>) =
    inherit ProductionWrapperBase(production)
    member x.SetReduceFunction (f : ('a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'T)) =
        production.SetReduceFunction (fun o -> box (f (o+>0) (o+>1) (o+>2) (o+>3) (o+>4) (o+>5)))

type ProductionWrapper<'a,'b,'c,'d,'e,'f,'g,'T> (production : IProduction<obj>) =
    inherit ProductionWrapperBase(production)
    member x.SetReduceFunction (f : ('a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g -> 'T)) =
        production.SetReduceFunction (fun o -> box (f (o+>0) (o+>1) (o+>2) (o+>3) (o+>4) (o+>5) (o+>6)))

type ProductionWrapper<'a,'b,'c,'d,'e,'f,'g,'h,'T> (production : IProduction<obj>) =
    inherit ProductionWrapperBase(production)
    member x.SetReduceFunction (f : ('a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g -> 'h -> 'T)) =
        production.SetReduceFunction (fun o -> box (f (o+>0) (o+>1) (o+>2) (o+>3) (o+>4) (o+>5) (o+>6) (o+>7)))

type ProductionWrapper<'a,'b,'c,'d,'e,'f,'g,'h,'i,'T> (production : IProduction<obj>) =
    inherit ProductionWrapperBase(production)
    member x.SetReduceFunction (f : ('a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g -> 'h -> 'i -> 'T)) =
        production.SetReduceFunction (fun o -> box (f (o+>0) (o+>1) (o+>2) (o+>3) (o+>4) (o+>5) (o+>6) (o+>7) (o+>8)))

type ProductionWrapper<'a,'b,'c,'d,'e,'f,'g,'h,'i,'j,'T> (production : IProduction<obj>) =
    inherit ProductionWrapperBase(production)
    member x.SetReduceFunction (f : ('a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g -> 'h -> 'i -> 'j -> 'T)) =
        production.SetReduceFunction (fun o -> box (f (o+>0) (o+>1) (o+>2) (o+>3) (o+>4) (o+>5) (o+>6) (o+>7) (o+>8)))
        
type ProductionWrapper<'a,'b,'c,'d,'e,'f,'g,'h,'i,'j,'k,'T> (production : IProduction<obj>) =
    inherit ProductionWrapperBase(production)
    member x.SetReduceFunction (f : ('a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g -> 'h -> 'i -> 'j -> 'k -> 'T)) =
        production.SetReduceFunction (fun o -> box (f (o+>0) (o+>1) (o+>2) (o+>3) (o+>4) (o+>5) (o+>6) (o+>7) (o+>8) (o+>9)))
        
type ProductionWrapper<'a,'b,'c,'d,'e,'f,'g,'h,'i,'j,'k,'l,'T> (production : IProduction<obj>) =
    inherit ProductionWrapperBase(production)
    member x.SetReduceFunction (f : ('a -> 'b -> 'c -> 'd -> 'e -> 'f -> 'g -> 'h -> 'i -> 'j -> 'k -> 'l -> 'T)) =
        production.SetReduceFunction (fun o -> box (f (o+>0) (o+>1) (o+>2) (o+>3) (o+>4) (o+>5) (o+>6) (o+>7) (o+>8) (o+>9) (o+>10)))

type SymbolWrapper<'T> (symbol : ISymbol<obj>) =
    member x.Symbol = symbol

type TerminalWrapper<'T> (terminal : ITerminal<obj>) =
    inherit SymbolWrapper<'T>(terminal)

type NonTerminalWrapper<'T> (nonTerminal : INonTerminal<obj>) =
    inherit SymbolWrapper<'T>(nonTerminal)
    
    let (!>) (p : SymbolWrapper<'a>) = p.Symbol;

    member x.AddProduction () =
        nonTerminal.AddProduction()
        |> ProductionWrapper<'T>
        
    member x.AddProduction p =
        nonTerminal.AddProduction !>p
        |> ProductionWrapper<'a,'T>
        
    member x.AddProduction (p1, p2) =
        nonTerminal.AddProduction(!>p1, !>p2)
        |> ProductionWrapper<'a,'b,'T>
        
    member x.AddProduction (p1, p2, p3) =
        nonTerminal.AddProduction(!>p1, !>p2, !>p3)
        |> ProductionWrapper<'a,'b,'c,'T>
        
    member x.AddProduction (p1, p2, p3, p4) =
        nonTerminal.AddProduction(!>p1, !>p2, !>p3, !>p4)
        |> ProductionWrapper<'a,'b,'c,'d,'T>
        
    member x.AddProduction (p1, p2, p3, p4, p5) =
        nonTerminal.AddProduction(!>p1, !>p2, !>p3, !>p4, !>p5)
        |> ProductionWrapper<'a,'b,'c,'d,'e,'T>
        
    member x.AddProduction (p1, p2, p3, p4, p5, p6) =
        nonTerminal.AddProduction(!>p1, !>p2, !>p3, !>p4, !>p5, !>p6)
        |> ProductionWrapper<'a,'b,'c,'d,'e,'f,'T>
        
    member x.AddProduction (p1, p2, p3, p4, p5, p6, p7) =
        nonTerminal.AddProduction(!>p1, !>p2, !>p3, !>p4, !>p5, !>p6, !>p7)
        |> ProductionWrapper<'a,'b,'c,'d,'e,'f,'g,'T>
        
    member x.AddProduction (p1, p2, p3, p4, p5, p6, p7, p8) =
        nonTerminal.AddProduction(!>p1, !>p2, !>p3, !>p4, !>p5, !>p6, !>p7, !>p8)
        |> ProductionWrapper<'a,'b,'c,'d,'e,'f,'g,'h,'T>

    member x.AddProduction (p1, p2, p3, p4, p5, p6, p7, p8, p9) =
        nonTerminal.AddProduction(!>p1, !>p2, !>p3, !>p4, !>p5, !>p6, !>p7, !>p8, !>p9)
        |> ProductionWrapper<'a,'b,'c,'d,'e,'f,'g,'h,'i,'T>
        
    member x.AddProduction (p1, p2, p3, p4, p5, p6, p7, p8, p9, p10) =
        nonTerminal.AddProduction(!>p1, !>p2, !>p3, !>p4, !>p5, !>p6, !>p7, !>p8, !>p9, !>p10)
        |> ProductionWrapper<'a,'b,'c,'d,'e,'f,'g,'h,'i,'j,'T>
        
    member x.AddProduction (p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11) =
        nonTerminal.AddProduction(!>p1, !>p2, !>p3, !>p4, !>p5, !>p6, !>p7, !>p8, !>p9, !>p10, !>p11)
        |> ProductionWrapper<'a,'b,'c,'d,'e,'f,'g,'h,'i,'j,'k,'T>
        
    member x.AddProduction (p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12) =
        nonTerminal.AddProduction(!>p1, !>p2, !>p3, !>p4, !>p5, !>p6, !>p7, !>p8, !>p9, !>p10, !>p11, !>p12)
        |> ProductionWrapper<'a,'b,'c,'d,'e,'f,'g,'h,'i,'j,'k,'l,'T>
