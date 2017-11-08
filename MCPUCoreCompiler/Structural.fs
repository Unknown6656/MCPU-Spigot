module MCPUCompiler.Core.SyntaxTree


type Identifier = string
type IdentifierRef = { Identifier : string; }
    with
        override x.ToString() = x.Identifier
type ArrayIdentifierRef =
    | Memory
    | IO
    override x.ToString() =
        match x with
        | Memory -> "mem"
        | IO     -> "io"
type TypeSpec =
    | Void
    | Bool
    | Int
    override x.ToString() =
        function
        | Void -> "void"
        | Bool -> "bool"
        | Int  -> "int"
       <| x
type VariableDeclaration = TypeSpec * Identifier
type Parameters = VariableDeclaration list
type BinaryOperator =
    | Or
    | Equal
    | NotEqual
    | LessEqual
    | Less
    | GreaterEqual
    | Greater
    | And
    | Add
    | Subtract
    | Multiply
    | Divide
    | Modulus
    | Power
    | Xor
    | Shr
    | Shl
    | Ror
    | Rol
    override x.ToString() =
        function
        | Or            -> "|"
        | Equal         -> "=="
        | NotEqual      -> "!="
        | LessEqual     -> "<="
        | Less          -> "<"
        | GreaterEqual  -> ">="
        | Greater       -> ">"
        | And           -> "&"
        | Add           -> "+"
        | Subtract      -> "-"
        | Power         -> "**"
        | Multiply      -> "*"
        | Divide        -> "/"
        | Modulus       -> "%"
        | Xor           -> "^"
        | Shr           -> ">>"
        | Shl           -> "<<"
        | Ror           -> ">>>"
        | Rol           -> "<<<"
       <| x
type UnaryOperator =
    | LogicalNegate
    | Negate
    | Not
    | Incr
    | Decr
    | Identity
    | IntCast
    | BoolCast
    override x.ToString() =
        function
        | LogicalNegate -> "!"
        | Negate        -> "-"
        | Not           -> "~"
        | Incr          -> "++"
        | Decr          -> "--"
        | Identity      -> "+"
        | IntCast       -> "(int)"
        | BoolCast      -> "(bool)"
       <| x
and Literal =
    | BoolLiteral of bool
    | IntLiteral of int
    override x.ToString() =
        function
        | BoolLiteral b   -> b.ToString()
        | IntLiteral i    -> i.ToString()
       <| x
type Expression =
    | ScalarAssignmentExpression of IdentifierRef * Expression
    | ScalarAssignmentOperatorExpression of IdentifierRef * BinaryOperator * Expression
    | ArrayAssignmentExpression of ArrayIdentifierRef * Expression * Expression
    | ArrayAssignmentOperatorExpression of ArrayIdentifierRef * Expression * BinaryOperator * Expression
    | TernaryExpression of Expression * Expression * Expression
    | BinaryExpression of Expression * BinaryOperator * Expression
    | UnaryExpression of UnaryOperator * Expression
    | IdentifierExpression of IdentifierRef
    | ArrayIdentifierExpression of ArrayIdentifierRef * Expression
    | FunctionCallExpression of Identifier * Arguments
    | ArraySizeExpression of ArrayIdentifierRef
    | LiteralExpression of Literal
    override x.ToString() =
        match x with
        | TernaryExpression(c, x, y) -> sprintf "(%A) ? (%A) : (%A)" c x y
        | ScalarAssignmentExpression(x, y) -> sprintf "%A = (%A)" x y
        | ScalarAssignmentOperatorExpression(x, o, y) -> sprintf "%A %A= (%A)" x o y
        | ArrayAssignmentExpression(x, n, y) -> sprintf "%A[%A] = (%A)" x n y
        | ArrayAssignmentOperatorExpression(x, n, o, y) -> sprintf "%A[%A] %A= (%A)" x n o y
        | BinaryExpression(x, o, y) -> sprintf "(%A) %A (%A)" x o y
        | ArraySizeExpression x -> sprintf "sizeof(%A)" x
        // TODO
and Arguments = Expression list
type ExpressionStatement =
    | Expression of Expression
    | Nop
    override x.ToString() =
        match x with
        | Expression e -> e.ToString() + ";"
        | Nop -> ";"
type LocalDeclarations = VariableDeclaration list
type Statement =
    | ExpressionStatement of ExpressionStatement
    | CompoundStatement of CompoundStatement
  //| ForStatement of ForStatement
    | IfStatement of IfStatement
    | WhileStatement of WhileStatement
    | ReturnStatement of Expression option
    | BreakStatement
    | HaltStatement
    | AbkStatement
    | InlineAssemblyStatement of string
    override x.ToString() =
        match x with
        | ExpressionStatement es -> es.ToString()
        // | CompoundStatement c -> sprintf "{\n%A%A}" (for d in fst c -> d.ToString() + "\n") (for d in snd c -> d.ToString() + "\n")
and CompoundStatement = LocalDeclarations * Statement list
and IfStatement = Expression * Statement * Statement option
and WhileStatement = Expression * Statement
//and ForStatement = ExpressionStatement * Expression * ExpressionStatement * Statement
type FunctionDeclaration = TypeSpec * Identifier * Parameters * CompoundStatement
type Declaration =
    | GlobalVariableDeclaration of VariableDeclaration
    | FunctionDeclaration of FunctionDeclaration
type Program = Declaration list

type BuildInFunctions =
    | Func__printi
    | Func__abs
    | Func__sign
    | Func__pow
    | Func__log2
    | Func__log10
    | Func__iodir
    | Func__exp
    override x.ToString() = function
                            | Func__printi -> "printi"
                            | Func__abs -> "abs"
                            | Func__sign -> "sign"
                            | Func__pow -> "pow"
                            | Func__log2 -> "log2"
                            | Func__log10 -> "log10"
                            | Func__iodir -> "iodir"
                            | Func__exp -> "exp"
                           <| x

type VariableType =
    {
        Type : TypeSpec
        IsArray : bool
    }
    override x.ToString() =
        x.Type.ToString().ToLower() + (if x.IsArray then "[]" else "")

