namespace MCPUCompiler.Core

open Piglet.Lexer
open Piglet.Parser
open SyntaxTree


type CompilerException(message : string) =
    inherit System.Exception(message)
    
[<AutoOpen>]
module Errors =
    let private (/>) n m = CompilerException <| sprintf "MCPU%03i: %s" n m

    let LexerError (e : LexerException) =        1/>sprintf "Lexing error: %s near '%s' on line %d" (e.Message) (e.LineContents) (e.LineNumber)
    let ParserError (e : ParseException) =       2/>sprintf "Parsing error: %s. Expected [%A], got [%A] on line %d near '%s'" (e.Message) (e.ExpectedTokens) (e.FoundToken) (e.LexerState.CurrentLineNumber) (e.LexerState.LastLexeme)
    let VariableAlreadyDefined a =               3/>sprintf "A variable named '%s' is already defined in this scope" a
    let CannotConvertType (a : VariableType) (b : VariableType) =
                                                 4/>sprintf "Cannot convert type '%A' to '%A'" a b
    let BinopCannotBeApplied (a : BinaryOperator) (b : VariableType) (c : VariableType) =
                                                 5/>sprintf "The operator '%A' cannot be applied to operands of type '%A' and '%A'" a b c
    let UnopCannotBeApplied (a : UnaryOperator) (b : VariableType) =
                                                 6/>sprintf "The operator '%A' cannot be applied to an operand of type '%A'" a b
    let TernopCannotBeApplied (a : VariableType) (b : VariableType) (c : VariableType) =
                                                 7/>sprintf "The ternary operator '?:' cannot be applied to operands of type '%A', '%A' and '%A'" a b c
    let NameDoesNotExist a =                     8/>sprintf "The name '%s' does not exist in the current context" a
    let InvalidArguments a b c d =               9/>sprintf "Call to function '%s' has some invalid arguments. Argument %i: Cannot convert from '%s' to '%s'" a b c d
    let WrongNumberOfArguments a b c =           10/>sprintf "Function '%s' takes %i arguments, but here was given %i" a b c
    let NoEnclosingLoop() =                      12/>"No enclosing loop out of which to break"
    let CannotApplyIndexing (a : VariableType) = 13/>sprintf "Cannot apply indexing with [] to an expression of type '%A'" a
    let FunctionAlreadyDefined a =               14/>sprintf "A function named '%s' is already defined" a
    let MissingEntryPoint() =                    15/>"Program does not contain a 'main' method suitable for an entry point"
    let GeneralError (a : exn) =                 16/>sprintf "A compiler error occured: %s\n%s" (a.Message) (a.StackTrace)
    let GeneratorError x =                       17/>sprintf "The assembly instruction could not be generated for the following object: %A" x
