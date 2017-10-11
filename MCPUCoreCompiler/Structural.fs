namespace MCPUCompiler


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
    | Identity
    | IntCast
    | BoolCast
    override x.ToString() =
        function
        | LogicalNegate -> "!"
        | Negate        -> "-"
        | Not           -> "~"
        | Identity      -> "+"
        | IntCast       -> "(int)"
        | BoolCast      -> "(bool)"
       <| x
and Literal =
    | BoolLiteral of bool
    | IntLiteral of int
    | HexLiteral of int
    override x.ToString() =
        function
        | BoolLiteral b   -> b.ToString()
        | IntLiteral i    -> i.ToString()
        | HexLiteral h    -> "0x" + h.ToString "x8"
       <| x
type Expression =
    | TernaryExpression of Expression * Expression * Expression
    | ScalarAssignmentExpression of IdentifierRef * Expression
    | ScalarAssignmentOperatorExpression of IdentifierRef * BinaryOperator * Expression
    | ArrayAssignmentExpression of ArrayIdentifierRef * Expression * Expression
    | ArrayAssignmentOperatorExpression of ArrayIdentifierRef * Expression * BinaryOperator * Expression
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
        // TODO
and Arguments = Expression list
type ExpressionStatement =
    | Expression of Expression
    | Nop
type LocalDeclarations = VariableDeclaration list
type Statement =
    | ExpressionStatement of ExpressionStatement
    | CompoundStatement of CompoundStatement
    | ForStatement of ForStatement
    | IfStatement of IfStatement
    | WhileStatement of WhileStatement
    | ReturnStatement of Expression option
    | BreakStatement
    | HaltStatement
    | AbkStatement
    | InlineAssemblyStatement of string
and CompoundStatement = LocalDeclarations * Statement list
and IfStatement = Expression * Statement * Statement option
and WhileStatement = Expression * Statement
and ForStatement = ExpressionStatement * Expression * ExpressionStatement * Statement
type FunctionDeclaration = TypeSpec * Identifier * Parameters * CompoundStatement
type Declaration =
    | GlobalVariableDeclaration of VariableDeclaration
    | FunctionDeclaration of FunctionDeclaration
type Program = Declaration list
