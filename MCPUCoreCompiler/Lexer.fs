module MCPUCompiler.Core.Lexer

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
let asmStatement              = nterm<string>()
let compoundStatement         = nterm<CompoundStatement>()
let optionalLocalDeclarations = nterm<VariableDeclaration list>()
let localDeclarations         = nterm<VariableDeclaration list>()
let localDeclaration          = nterm<VariableDeclaration>()
let ifStatement               = nterm<IfStatement>()
let optionalElseStatement     = nterm<Statement option>()
let returnStatement           = nterm<Expression option>()
let breakStatement            = nterm<unit>()
let haltStatement             = nterm<unit>()
let abkStatement              = nterm<unit>()
let expression                = nterm<Expression>()
let binaryAssignOperator      = nterm<BinaryOperator>()
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
let rotateRight      = term  ">>>"
let shiftRight       = term  ">>"
let rotateLeft       = term  "<<<"
let shiftLeft        = term  "<<"
let ifKeyword        = term  "if"
let elseKeyword      = term  "else"
let whileKeyword     = term  "while"
let returnKeyword    = term  "return"
let breakKeyword     = term  "break"
let haltKeyword      = term  "halt"
let abkKeyword       = term  "abk"
let forKeyword       = term  "for"
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
let ioLiteral        = termf @"(in|out)"                                (fun s -> BoolLiteral(s.ToLower() <> "in"))
let intLiteral       = termf @"\d+"                                     (fun s -> IntLiteral(int32 s))
let hexLiteral       = termf @"(0(x|X)[0-9a-fA-F]+|[0-9a-fA-F]+(h|H))"  (fun s -> IntLiteral(System.Int32.Parse(s, NumberStyles.HexNumber)))
let trueLiteral      = termf "true"                                     (fun _ -> BoolLiteral true)
let falseLiteral     = termf "false"                                    (fun _ -> BoolLiteral false)
let boolKeyword      = termf "bool"                                     (fun _ -> Bool)
let intKeyword       = termf "int"                                      (fun _ -> Int)
let identifier       = termf "[a-zA-Z_][a-zA-Z_0-9]*"                   id
let arridentifier    = termf "(io|mem)"                                 (fun s -> if s.ToLower() = "io" then IO else Memory)
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
let doubleQuote      = term  "\""
let asm              = termf "[^\"]+" id


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
assoc Right [ questionmark ] // I'm not sure about this line
assoc Right [ AssignAdd; AssignAnd; AssignDivide; AssignModulo; AssignMultiply; AssignOr; AssignPower; AssignRotateLeft;
              AssignRotateRight; AssignShiftLeft; AssignShiftRight; AssignSubtract; AssignXor; ]
assoc Left [ singleEquals ]
assoc Left [ hat ]
assoc Left [ pipe ]
assoc Left [ ampersand ]
assoc Left [ doubleEquals; doublePipes ]
assoc Left [ openAngle; openAngleEquals; closeAngle; closeAngleEquals ]
assoc Left [ rotateLeft; shiftLeft; rotateRight; shiftRight ]
assoc Left [ tilde; minus; plus; exclamation; doublePlus; doubleMinus ]
assoc Left [ asterisk; forwardSlash; percent ]
assoc Right [ doubleAsterisk ]
assoc Right [ CastBool; CastInt ]
assoc Right [ doubleMinus; doublePlus; ]

let unaryExpressionPrecedenceGroup  = conf.RightAssociative()

let reducef (s : NonTerminalWrapper<'a>) x = s.AddProduction().SetReduceFunction x
let reduce0 (s : NonTerminalWrapper<'a>) a = s.AddProduction(a).SetReduceToFirst()
let reduce1 (s : NonTerminalWrapper<'a>) a x = s.AddProduction(a).SetReduceFunction x
let reduce2 (s : NonTerminalWrapper<'a>) a b x = s.AddProduction(a, b).SetReduceFunction x
let reduce3 (s : NonTerminalWrapper<'a>) a b c x = s.AddProduction(a, b, c).SetReduceFunction x
let reduce4 (s : NonTerminalWrapper<'a>) a b c d x = s.AddProduction(a, b, c, d).SetReduceFunction x
let reduce5 (s : NonTerminalWrapper<'a>) a b c d e x = s.AddProduction(a, b, c, d, e).SetReduceFunction x
let reduce6 (s : NonTerminalWrapper<'a>) a b c d e f x = s.AddProduction(a, b, c, d, e, f).SetReduceFunction x
let reduce7 (s : NonTerminalWrapper<'a>) a b c d e f g x = s.AddProduction(a, b, c, d, e, f, g).SetReduceFunction x
let reduce8 (s : NonTerminalWrapper<'a>) a b c d e f g h x = s.AddProduction(a, b, c, d, e, f, g, h).SetReduceFunction x
let reducelist (listtype : NonTerminalWrapper<'a list>) separator element =
    reduce3 listtype listtype separator element (fun l _ e -> l @ [e])
    reduce1 listtype element (fun e -> [e])
let reduceclist (listtype : NonTerminalWrapper<'a list>) element =
    reduce2 listtype listtype element (fun l e -> l @ [e])
    reduce1 listtype element (fun e -> [e])


reduce0 program declarationList

reduceclist declarationList declaration

reduce1 declaration staticVariableDeclaration GlobalVariableDeclaration
reduce1 declaration functionDeclaration FunctionDeclaration

reduce1 typeSpec voidKeyword (fun _ -> Void)
reduce0 typeSpec boolKeyword
reduce0 typeSpec intKeyword

reduce3 staticVariableDeclaration typeSpec identifier semicolon (fun t i _ -> (t, i))
reduce6 functionDeclaration typeSpec identifier openParen parameters closeParen compoundStatement (fun t i _ p _ c -> (t, i, p, c))

reduce0 parameters parameterList
reduce1 parameters voidKeyword (fun _ -> [])

reducelist parameterList comma parameter

reduce2 parameter typeSpec identifier (fun t i -> (t, i))

reduce0 optionalStatementList statementList
reducef optionalStatementList (fun () -> [])

reduceclist statementList statement

reduce1 statement expressionStatement ExpressionStatement
reduce1 statement compoundStatement CompoundStatement
reduce1 statement ifStatement IfStatement
reduce1 statement whileStatement WhileStatement
reduce1 statement returnStatement ReturnStatement
reduce1 statement breakStatement (fun _ -> BreakStatement)
reduce1 statement haltStatement (fun _ -> HaltStatement)
reduce1 statement abkStatement (fun _ -> AbkStatement)
reduce1 statement asmStatement InlineAssemblyStatement

reduce2 expressionStatement expression semicolon (fun e _ -> Expression e)
reduce1 expressionStatement semicolon (fun _ -> Nop)

reduce4 compoundStatement openCurly optionalLocalDeclarations optionalStatementList closeCurly (fun _ l s _ -> (l, s))
reduce8 compoundStatement forKeyword openParen expressionStatement expression semicolon expression closeParen statement (fun _ _ f t _ o _ s ->
    (
        [], [
            ExpressionStatement f
            WhileStatement (
                                t,
                                CompoundStatement ([], [
                                                            s
                                                            (Expression >> ExpressionStatement) o
                                                       ])
                           )
        ]
    ))
    
reduce5 whileStatement whileKeyword openParen expression closeParen statement (fun _ _ e _ s -> (e, s))
reduce6 whileStatement forKeyword openParen semicolon semicolon closeParen statement (fun _ _ _ _ _ s -> (LiteralExpression(BoolLiteral true), s))

reduce0 optionalLocalDeclarations localDeclarations
reducef optionalLocalDeclarations (fun () -> [])

reduceclist localDeclarations localDeclaration

reduce3 localDeclaration typeSpec identifier semicolon (fun t i _ -> (t, i))

reduce6 ifStatement ifKeyword openParen expression closeParen statement optionalElseStatement (fun _ _ e _ s o -> (e, s, o))

let elseStatementProduction = optionalElseStatement.AddProduction(elseKeyword, statement)
let elseEpsilonProduction = optionalElseStatement.AddProduction()

elseStatementProduction.SetReduceFunction (fun _ b -> Some b)
elseStatementProduction.SetPrecedence prec_optelse

elseEpsilonProduction.SetReduceFunction (fun () -> None)
elseEpsilonProduction.SetPrecedence prec_optelse

reduce3 returnStatement returnKeyword expression semicolon (fun _ e _ -> Some e)
reduce2 returnStatement returnKeyword semicolon (fun _ _ -> None)

reduce2 breakStatement breakKeyword semicolon (fun _ _ -> ())
reduce2 haltStatement haltKeyword semicolon (fun _ _ -> ())
reduce2 abkStatement abkKeyword semicolon (fun _ _ -> ())
reduce5 asmStatement asmKeyword doubleQuote asm doubleQuote semicolon (fun _ _ a _ _ -> a)

reduce1 binaryAssignOperator AssignAdd (fun _ -> Add)
reduce1 binaryAssignOperator AssignAnd (fun _ -> And)
reduce1 binaryAssignOperator AssignDivide (fun _ -> Divide)
reduce1 binaryAssignOperator AssignModulo (fun _ -> Modulus)
reduce1 binaryAssignOperator AssignMultiply (fun _ -> Multiply)
reduce1 binaryAssignOperator AssignOr (fun _ -> Or)
reduce1 binaryAssignOperator AssignPower (fun _ -> Power)
reduce1 binaryAssignOperator AssignRotateLeft (fun _ -> Rol)
reduce1 binaryAssignOperator AssignRotateRight (fun _ -> Ror)
reduce1 binaryAssignOperator AssignShiftLeft (fun _ -> Shl)
reduce1 binaryAssignOperator AssignShiftRight (fun _ -> Shr)
reduce1 binaryAssignOperator AssignSubtract (fun _ -> Subtract)
reduce1 binaryAssignOperator AssignXor (fun _ -> Xor)

reduce3 expression identifier singleEquals expression (fun i _ e -> ScalarAssignmentExpression({ Identifier = i }, e))
reduce6 expression arridentifier openSquare expression closeSquare singleEquals expression (fun i _ n _ _ e -> ArrayAssignmentExpression(i, n, e))
reduce3 expression identifier binaryAssignOperator expression (fun i o e -> ScalarAssignmentOperatorExpression({ Identifier = i}, o, e))
reduce6 expression arridentifier openSquare expression closeSquare binaryAssignOperator expression (fun i _ n _ o e -> ArrayAssignmentOperatorExpression(i, n, o, e))
reduce3 expression expression doublePipes expression (fun x _ y -> BinaryExpression(x, Or, y))
reduce3 expression expression pipe expression (fun x _ y -> BinaryExpression(x, Or, y))
reduce3 expression expression doubleAmpersands expression (fun x _ y -> BinaryExpression(x, And, y))
reduce3 expression expression ampersand expression (fun x _ y -> BinaryExpression(x, And, y))
reduce3 expression expression rotateLeft expression (fun x _ y -> BinaryExpression(x, Rol, y))
reduce3 expression expression shiftLeft expression (fun x _ y -> BinaryExpression(x, Shl, y))
reduce3 expression expression rotateRight expression (fun x _ y -> BinaryExpression(x, Ror, y))
reduce3 expression expression shiftRight expression (fun x _ y -> BinaryExpression(x, Shr, y))
reduce3 expression expression plus expression (fun x _ y -> BinaryExpression(x, Add, y))
reduce3 expression expression minus expression (fun x _ y -> BinaryExpression(x, Subtract, y))
reduce3 expression expression asterisk expression (fun x _ y -> BinaryExpression(x, Multiply, y))
reduce3 expression expression doubleAsterisk expression (fun x _ y -> BinaryExpression(x, Power, y))
reduce3 expression expression hat expression (fun x _ y -> BinaryExpression(x, Xor, y))
reduce3 expression expression percent expression (fun x _ y -> BinaryExpression(x, Modulus, y))
reduce3 expression expression forwardSlash expression (fun x _ y -> BinaryExpression(x, Divide, y))
reduce3 expression expression doubleEquals expression (fun x _ y -> BinaryExpression(x, Equal, y))
reduce3 expression expression bangEquals expression (fun x _ y -> BinaryExpression(x, NotEqual, y))
reduce3 expression expression openAngleEquals expression (fun x _ y -> BinaryExpression(x, LessEqual, y))
reduce3 expression expression openAngle expression (fun x _ y -> BinaryExpression(x, Less, y))
reduce3 expression expression closeAngleEquals expression (fun x _ y -> BinaryExpression(x, GreaterEqual, y))
reduce3 expression expression closeAngle expression (fun x _ y -> BinaryExpression(x, Greater, y))
reduce5 expression expression questionmark expression colon expression (fun c _ x _ y -> TernaryExpression(c, x, y))

let unaryExpressionProduction = expression.AddProduction(unaryOperator, expression)
unaryExpressionProduction.SetReduceFunction (fun a b -> UnaryExpression(a, b))
unaryExpressionProduction.SetPrecedence unaryExpressionPrecedenceGroup

reduce3 expression openParen expression closeParen (fun _ e _ -> e)
reduce1 expression identifier (fun i -> IdentifierExpression({ Identifier = i }))
reduce4 expression arridentifier openSquare expression closeSquare (fun i _ e _ -> ArrayIdentifierExpression(i, e))
reduce4 expression identifier openParen optionalArguments closeParen (fun i _ a _ -> FunctionCallExpression(i, a))
reduce3 expression arridentifier period sizeKeyword (fun i _ _ -> ArraySizeExpression i)
reduce1 expression trueLiteral LiteralExpression
reduce1 expression falseLiteral LiteralExpression
reduce1 expression intLiteral LiteralExpression
reduce1 expression hexLiteral LiteralExpression
reduce1 expression ioLiteral LiteralExpression

reduce1 unaryOperator exclamation (fun _ -> LogicalNegate)
reduce1 unaryOperator doubleMinus (fun _ -> Decr)
reduce1 unaryOperator minus (fun _ -> Negate)
reduce1 unaryOperator doublePlus (fun _ -> Incr)
reduce1 unaryOperator plus (fun _ -> Identity)
reduce1 unaryOperator tilde (fun _ -> Not)
reduce1 unaryOperator CastInt (fun _ -> IntCast)
reduce1 unaryOperator CastBool (fun _ -> BoolCast)

reduce0 optionalArguments arguments
reducef optionalArguments (fun () -> [])

reducelist arguments comma expression

conf.LexerSettings.Ignore <- [| @"\s+"; @"/\*(.|[\r\n])*?\*/"; @"//[^\n]*\n" |]


let parser = conf.CreateParser()

let parse (s : string) =
    try
        parser.Parse(s) :?> Program
    with
        | :? Piglet.Lexer.LexerException as ex -> raise <| LexerError ex.Message
        | :? Piglet.Parser.ParseException as ex -> raise <| ParserError ex.Message
        | ex -> raise <| GeneralError ex