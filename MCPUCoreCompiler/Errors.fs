namespace MCPUCompiler.Core
open System.Text.RegularExpressions

open Piglet.Lexer
open Piglet.Parser
open SyntaxTree


type CompilerException(inner : System.Exception, message : string) =
    inherit System.Exception(message, inner)
    
[<AutoOpen>]
module Errors =
    let private (/>>) n (m, e) = CompilerException <| (e, sprintf "MCPU%03i: %s" n m)
    let private (/>) n m = n />> (m, null)
    let private replwsp s = Regex.Replace(s, @"(\s+|\r|\n)", " ").Trim();

    let LexerError (e : LexerException) =        1/>>(sprintf "Lexing error: %s near '%s' on line %d." (replwsp e.Message) (e.LineContents) (e.LineNumber), e)
    let ParserError (e : ParseException) =       2/>>(sprintf "Parsing error: %s. Expected [%A], got [%A] on line %d near '%s'." (replwsp e.Message) (e.ExpectedTokens) (e.FoundToken) (e.LexerState.CurrentLineNumber) (replwsp e.LexerState.LastLexeme), e)
    let VariableAlreadyDefined a =               3/>sprintf "A variable named '%s' is already defined in this scope." a
    let CannotConvertType (a : VariableType) (b : VariableType) =
                                                 4/>sprintf "Cannot convert type '%A' to '%A'" a b
    let BinopCannotBeApplied (a : BinaryOperator) (b : VariableType) (c : VariableType) =
                                                 5/>sprintf "The operator '%A' cannot be applied to operands of type '%A' and '%A'." a b c
    let UnopCannotBeApplied (a : UnaryOperator) (b : VariableType) =
                                                 6/>sprintf "The operator '%A' cannot be applied to an operand of type '%A'." a b
    let TernopCannotBeApplied (a : VariableType) (b : VariableType) (c : VariableType) =
                                                 7/>sprintf "The ternary operator '?:' cannot be applied to operands of type '%A', '%A' and '%A'." a b c
    let NameDoesNotExist a =                     8/>sprintf "The name '%s' does not exist in the current context." a
    let InvalidArguments a b c d =               9/>sprintf "Call to function '%s' has some invalid arguments. Argument %i: Cannot convert from '%s' to '%s'." a b c d
    let WrongNumberOfArguments a b c =           10/>sprintf "Function '%s' takes %i arguments, but here was given %i." a b c
    let NoEnclosingLoop() =                      12/>"No enclosing loop out of which to break or continue."
    let CannotApplyIndexing (a : VariableType) = 13/>sprintf "Cannot apply indexing with [] to an expression of type '%A'." a
    let FunctionAlreadyDefined a =               14/>sprintf "A function named '%s' is already defined." a
    let MissingEntryPoint() =                    15/>"Program does not contain a 'main' method suitable for an entry point."
    let GeneralError (a : exn) =                 16/>>("A compiler error occured.", a)
    let CompilerASTError =                       17/>"The abstract syntax tree could not be built."
    let GeneratorError x =                       18/>sprintf "The assembly instruction could not be generated for the following object: %A." x
    let MainCannotBeInlined =                    19/>"The function 'main' cannot be inlined as it is the entry point function."
    let CannotBeInlined a =                      20/>sprintf "The function '%s' cannot be inlined as it contains an other function call." a
