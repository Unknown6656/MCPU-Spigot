module MCPUCompiler.Core.SyntaxTree

let inline (!/) x = x.ToString()

type IdentifierRef =
    {
        Identifier : string;
    }
    with
        override x.ToString() = x.Identifier
type Identifier = string
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
        match x with
        | BoolLiteral b -> if b then "true" else "false"
        | IntLiteral i -> !/i
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
    | UUIDExpression
    override x.ToString() =
        match x with
        | TernaryExpression(c, x, y) -> sprintf "(%s) ? (%s) : (%s)" !/c !/x !/y
        | ScalarAssignmentExpression(x, y) -> sprintf "%s = (%s)" !/x !/y
        | ScalarAssignmentOperatorExpression(x, o, y) -> sprintf "%s %s= (%s)" !/x !/o !/y
        | ArrayAssignmentExpression(x, n, y) -> sprintf "%s[%s] = (%s)" !/x !/n !/y
        | ArrayAssignmentOperatorExpression(x, n, o, y) -> sprintf "%s[%s] %s= (%s)" !/x !/n !/o !/y
        | BinaryExpression(x, o, y) -> sprintf "(%s) %s (%s)" !/x !/o !/y
        | ArraySizeExpression x -> sprintf "sizeof(%s)" !/x
        | LiteralExpression l -> !/l
        | ArrayIdentifierExpression(x, y) -> sprintf "%s[%s]" !/x !/y
        | FunctionCallExpression(x, y) -> sprintf "%s(%s)" x (System.String.Join(", ", y))
        | IdentifierExpression i -> i.Identifier
        | UnaryExpression(o, x) -> sprintf "%s(%s)" !/o !/x
        | UUIDExpression -> "__uuidof(this)"
and Arguments = Expression list
type ExpressionStatement =
    | Expression of Expression
    | Nop
    override x.ToString() =
        match x with
        | Expression e -> !/e + ";"
        | Nop -> ";"
type LocalDeclarations = VariableDeclaration list
type Statement =
    | ExpressionStatement of ExpressionStatement
    | CompoundStatement of CompoundStatement
    | IfStatement of IfStatement
    | WhileStatement of WhileStatement
    | ReturnStatement of Expression option
    | BreakStatement
    | ContinueStatement
    | HaltStatement
    | AbkStatement
    | InlineAssemblyStatement of string
    override x.ToString() =
        match x with
        | AbkStatement -> "abk;"
        | HaltStatement -> "halt;"
        | BreakStatement -> "break;"
        | ContinueStatement -> "continue;"
        | ReturnStatement None -> "return;"
        | ReturnStatement (Some s) -> "return " + !/s + ";"
        | ExpressionStatement es -> !/es
        | IfStatement (e, s1, None) -> sprintf "if (%s)\n{\n%s\n}" !/e !/s1
        | IfStatement (e, s1, Some s2) -> sprintf "if (%s)\n{\n%s\n}\nelse\n{\n%s\n}" !/e !/s1 !/s2
        | InlineAssemblyStatement s -> sprintf "__asm \"%s\";" s
        | WhileStatement (e, s) -> sprintf "while (%s)\n{\n%s\n}" !/e !/s
        | CompoundStatement (l, s) ->
            let pline o f = String.concat "" (List.toSeq [for d in o -> f d + "\n"])
            let vars = pline l (fun (t, i) -> sprintf "%s %s;" !/t !/i)
            let ins = pline s (!/)
            sprintf "{\n%s%s}" vars ins
and CompoundStatement = LocalDeclarations * Statement list
and IfStatement = Expression * Statement * Statement option
and WhileStatement = Expression * Statement
type FunctionDeclaration = TypeSpec * Identifier * Parameters * CompoundStatement * bool
type Declaration =
    | GlobalVariableDeclaration of VariableDeclaration
    | FunctionDeclaration of FunctionDeclaration
    override x.ToString() =
        match x with
        | GlobalVariableDeclaration(t, i) -> sprintf "%s %s;" !/t !/i
        | FunctionDeclaration(r, i, p, c, l) -> sprintf "%s%s %s(%s)\n%s" (if l then "inline " else "") !/r !/i (String.concat ", " ([for (t, i) in p -> !/t + " " + !/i]
                                                                                                                 |> List.toSeq)) !/(CompoundStatement c)
type Program = Declaration list

type BuildInFunctions =
    | Func__printi
    | Func__abs
    | Func__min
    | Func__max
    | Func__sign
    | Func__pow
    | Func__log2
    | Func__log10
    | Func__iodir
    | Func__exp
    override x.ToString() = function
                            | Func__printi -> "printi"
                            | Func__min -> "min"
                            | Func__max -> "max"
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

