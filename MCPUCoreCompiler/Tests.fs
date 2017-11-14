module MCPUCompiler.Core.Testing.Tests

open MCPUCompiler.Core.SyntaxTree;

let internal var t i = (t, i) : VariableDeclaration
let internal field t i = GlobalVariableDeclaration(t, i)
let internal (/-->) (t, i, p) c = FunctionDeclaration(t, i, p, c)
let internal idref s = IdentifierExpression { Identifier = s }


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
