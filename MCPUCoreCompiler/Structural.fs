namespace MCPUCompiler


type Identifier = string
type IdentifierRef = { Identifier : string; }
type ArrayIdentifierRef =
    | Memory
    | IO
type TypeSpec =
    | Void
    | Bool
    | Int
    override x.ToString() =
        function
        | Void  -> "void"
        | Bool  -> "bool"
        | Int   -> "int"
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
    | ArrayAssignmentOperatorExpression of IdentifierRef * BinaryOperator * Expression
    | BinaryExpression of Expression * BinaryOperator * Expression
    | UnaryExpression of UnaryOperator * Expression
    | IdentifierExpression of IdentifierRef
    | ArrayIdentifierExpression of ArrayIdentifierRef * Expression
    | FunctionCallExpression of Identifier * Arguments
    | ArraySizeExpression of ArrayIdentifierRef
    | LiteralExpression of Literal
and Arguments = Expression list
type ExpressionStatement =
    | Expression of Expression
    | Nop
type LocalDeclarations = VariableDeclaration list
type Statement =
    | ExpressionStatement of ExpressionStatement
    | CompoundStatement of CompoundStatement
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
type FunctionDeclaration = TypeSpec * Identifier * Parameters * CompoundStatement
type Declaration =
    | StaticVariableDeclaration of VariableDeclaration
    | FunctionDeclaration of FunctionDeclaration
type Program = Declaration list
