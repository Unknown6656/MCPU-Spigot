module MCPUCompiler.Parser

open System.Globalization
open Piglet.Parser
open Util


let conf = ParserFactory.Configure<obj>()
let nterm<'T> () = NonTerminalWrapper<'T> (conf.CreateNonTerminal())
let termf<'T> regex (onParse : (string -> 'T)) = TerminalWrapper<'T> (conf.CreateTerminal(regex, (fun s -> box (onParse s))))
let term = conf.CreateTerminal >> TerminalWrapper<string>


let program                   = nterm<Program>()
let declarationList           = nterm<Declaration list>()
let declaration               = nterm<Declaration>()
let staticVariableDeclaration = nterm<VariableDeclaration>()
let functionDeclaration       = nterm<FunctionDeclaration>()
let typeSpec                  = nterm<TypeSpec>()
let parameters                = nterm<Parameters>()
let parameterList             = nterm<Parameters>()
let parameter                 = nterm<VariableDeclaration>()
let optionalStatementList     = nterm<Statement list>()
let statementList             = nterm<Statement list>()
let statement                 = nterm<Statement>()
let expressionStatement       = nterm<ExpressionStatement>()
let whileStatement            = nterm<WhileStatement>()
let compoundStatement         = nterm<CompoundStatement>()
let optionalLocalDeclarations = nterm<VariableDeclaration list>()
let localDeclarations         = nterm<VariableDeclaration list>()
let localDeclaration          = nterm<VariableDeclaration>()
let ifStatement               = nterm<IfStatement>()
let optionalElseStatement     = nterm<Statement option>()
let returnStatement           = nterm<Expression option>()
let breakStatement            = nterm<unit>()
let expression                = nterm<Expression>()
let unaryOperator             = nterm<UnaryOperator>()
let optionalArguments         = nterm<Arguments>()
let arguments                 = nterm<Arguments>()

let CastInt          = term "(int)"
let CastBool         = term "(bool)"
let AssignRotateRight = term ">>>="
let AssignShiftRight = term  ">>="
let AssignRotateLeft = term  "<<<="
let AssignShiftLeft  = term  "<<="
let AssignAdd        = term  @"\+="
let AssignSubtract   = term  "-="
let AssignMultiply   = term  @"\*="
let AssignPower      = term  @"\*\*="
let AssignDivide     = term  "/="
let AssignModulo     = term  "%="
let AssignOr         = term  @"\|="
let AssignXor        = term  @"\^="
let AssignAnd        = term  "&="
let RotateRight      = term  ">>>"
let ShiftRight       = term  ">>"
let RotateLeft       = term  "<<<"
let ShiftLeft        = term  "<<"
let ifKeyword        = term  "if"
let elseKeyword      = term  "else"
let whileKeyword     = term  "while"
let returnKeyword    = term  "return"
let breakKeyword     = term  "break"
let haltKeyword      = term  "halt"
let abkKeyword       = term  "abk"
let newKeyword       = term  "new"
let sizeKeyword      = term  "length"
let asmKeyword       = term  "__asm"
let voidKeyword      = term  "void"
let plus             = term  @"\+"
let minus            = term  "-"
let doubleMinus      = term  "--"
let doublePlus       = term  @"\+\+"
let tilde            = term  "~"
let exclamation      = term  "!"
let questionmark     = term  "?"
let colon            = term  ":"
let asterisk         = term  @"\*"
let doubleAsterisk   = term  @"\*\*"
let hat              = term  @"\^"
let intLiteral       = termf @"\d+"                                     (fun s -> IntLiteral(int32 s))
let hexLiteral       = termf @"(0(x|X)[0-9a-fA-F]+|[0-9a-fA-F]+(h|H))"  (fun s -> IntLiteral(System.Int32.Parse(s, NumberStyles.HexNumber)))
let trueLiteral      = termf "true"                                     (fun _ -> BoolLiteral true)
let falseLiteral     = termf "false"                                    (fun _ -> BoolLiteral false)
let boolKeyword      = termf "bool"                                     (fun _ -> Bool)
let intKeyword       = termf "int"                                      (fun _ -> Int)
let identifier       = termf "[a-zA-Z_][a-zA-Z_0-9]*"                   id
let openParen        = term  @"\("
let closeParen       = term  @"\)"
let openCurly        = term  @"\{"
let closeCurly       = term  @"\}"
let openSquare       = term  @"\["
let closeSquare      = term  @"\]"
let semicolon        = term  ";"
let comma            = term  ","
let percent          = term  "%"
let forwardSlash     = term  "/"
let singleEquals     = term  "="
let pipe             = term  @"\|"
let doublePipes      = term  @"\|\|"
let doubleEquals     = term  "=="
let bangEquals       = term  "!="
let openAngleEquals  = term  "<="
let openAngle        = term  "<"
let closeAngleEquals = term  ">="
let closeAngle       = term  ">"
let ampersand        = term  "&"
let doubleAmpersands = term  "&&"
let period           = term  @"\."


type OpetatorAssociativity = Left | Right

let prec_optelse = conf.LeftAssociative()
let assoc d x =
    let arg = List.map (fun (f : SymbolWrapper<_>) -> downcast f.Symbol)
            >> List.toArray
    match d with
    | Left -> conf.LeftAssociative(arg x)
    | Right -> conf.RightAssociative(arg x)
    |> ignore
    
assoc Left [ elseKeyword ]
assoc Left [ colon ] // I'm not sure about this line
assoc Right [ AssignAdd; AssignAnd; AssignDivide; AssignModulo; AssignMultiply; AssignOr; AssignPower; AssignRotateLeft;
              AssignRotateRight; AssignShiftLeft; AssignShiftRight; AssignSubtract; AssignXor; ]
assoc Left [ singleEquals ]
assoc Left [ hat ]
assoc Left [ pipe ]
assoc Left [ ampersand ]
assoc Left [ doubleEquals; doublePipes ]
assoc Left [ openAngle; openAngleEquals; closeAngle; closeAngleEquals ]
assoc Left [ RotateLeft; ShiftLeft; RotateRight; ShiftRight ]
assoc Left [ tilde; minus; plus; exclamation ]
assoc Left [ asterisk; forwardSlash; percent ]
assoc Right [ doubleAsterisk ]
assoc Right [ CastBool; CastInt ]
assoc Right [ doubleMinus; doublePlus; ]


let reducef (s : NonTerminalWrapper<'a>) x = s.AddProduction().SetReduceFunction x
let reduce0 (s : NonTerminalWrapper<'a>) a = s.AddProduction(a).SetReduceToFirst()
let reduce1 (s : NonTerminalWrapper<'a>) a x = s.AddProduction(a).SetReduceFunction x
let reduce2 (s : NonTerminalWrapper<'a>) a b x = s.AddProduction(a, b).SetReduceFunction x
let reduce3 (s : NonTerminalWrapper<'a>) a b c x = s.AddProduction(a, b, c).SetReduceFunction x
let reduce4 (s : NonTerminalWrapper<'a>) a b c d x = s.AddProduction(a, b, c, d).SetReduceFunction x
let reduce5 (s : NonTerminalWrapper<'a>) a b c d e x = s.AddProduction(a, b, c, d, e).SetReduceFunction x
let reduce6 (s : NonTerminalWrapper<'a>) a b c d e f x = s.AddProduction(a, b, c, d, e, f).SetReduceFunction x


