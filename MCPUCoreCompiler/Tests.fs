module MCPUCompiler.Core.Testing.Tests

open MCPUCompiler.Core.SyntaxTree;

let internal var t i = (t, i) : VariableDeclaration
let internal field t i = GlobalVariableDeclaration(t, i)
let internal (/-->) (t, i, p) c = FunctionDeclaration(t, i, p, c, false)
let internal idref s = IdentifierExpression { Identifier = s }


let Code01 = @"
void main(void)
{
}
"
let Code02 = @"
int g1;
bool g2;

void main(int p1, bool p2)
{
    bool g2;
}

void helper(bool p1)
{
    int l1;
}
"
let Code03 = @"
void main(void)
{
    __asm ""nop"";
    __asm ""test:"";
    __asm ""abk"";
    __asm ""br test"";
}
"
let Code04 = @"
void main(void)
{
    int i;

    i = 0;

    while (i < 10)
    {
        printi(i);

        i = i + 1;
    }
}
"
let Code05 = @"
void main(void)
{
    int i;

    for (i = 0; i < 9; i = i + 1)
    {
        printi(i);
    }
}
"
let Code06 = @"
void main(void)
{
    for each (int k -> int v in io)
    {
        printi(k);
        printi(v);
    }
}
"

let Tree01 = [
                 field Int "glob_1"
                 field Bool "glob_2"
                 (Void, "main", []) /--> ([], [])
             ]
let Tree02 = [
                 field Int "glob_1"
                 field Bool "glob_2"
                 (Bool, "helper", [var Int "p"]) /--> ([], [
                                                                ReturnStatement(
                                                                    Some(
                                                                        LiteralExpression(
                                                                            BoolLiteral false
                                                                        )
                                                                    )
                                                                )
                                                           ])
                 (Void, "main", [var Int "p1"; var Bool "p2"]) /--> ([var Int "test"], [])
             ]
let Tree03 = [
                 (Void, "main", []) /--> ([
                                            var Int "loc1"
                                            var Int "loc2"
                                            var Bool "loc3"
                                          ],
                                          [
                                            IfStatement (
                                                idref "loc3",
                                                ExpressionStatement(
                                                    Expression(
                                                        ScalarAssignmentExpression(
                                                            { Identifier = "loc1" },
                                                            idref "loc2"
                                                        )
                                                    )
                                                ),
                                                None
                                            )
                                          ])
             ]
let Tree04 = [
                 field Int "glob_1"
                 field Bool "glob_2"
                 (Void, "main", []) /--> ([],
                                          [
                                            InlineAssemblyStatement("nop")
                                            AbkStatement
                                            HaltStatement
                                            WhileStatement (
                                                LiteralExpression (
                                                    BoolLiteral true
                                                ),
                                                BreakStatement
                                            )
                                          ])
             ]
