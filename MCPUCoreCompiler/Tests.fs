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
