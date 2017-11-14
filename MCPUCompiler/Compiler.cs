#define FAIL_HARD

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCPUCompiler
{
    using MCPUCompiler.Core;
    using MCPUCompiler.Core.Testing;
    using __comp = Core.Compiler;


    public sealed class Compiler
        : IDisposable
    {
        public string[] InstructionSet { get; }


        public Compiler(string[] instructions) => InstructionSet = instructions;

        public void Dispose()
        {

        }

        public CompilerResult Compile(string code)
        {
#if !FAIL_HARD
            try
#endif
            {
                List<Action> funcs = new List<Action>();

                // var tree = Tests.Tree03;
                var tree = Lexer.Parse(code, true);
                var sares = Parser.Analyze(tree);
                var compl = new __comp.ASMBuilder(sares);
                var res = compl.BuildClass(tree);
                var emt = new Emitter(res);
                var asm = emt.Merge();
                var outp = emt.Generate();
                var acode = string.Join("\n", outp); // join, as some 'lines' can have line-breaks in themselves

                funcs.Add(() =>
                {
                    int ind = 0;
                    var treelines = from c in tree
                                    from l in c.ToString().Split('\n')
                                    select new Func<string>(() =>
                                    {
                                        string line = l.Trim();

                                        if (line == "{")
                                        {
                                            ++ind;

                                            return new string(' ', (ind - 1) * 4) + line;
                                        }
                                        else if (line == "}")
                                            --ind;

                                        return new string(' ', ind * 4) + line;
                                    })();
                    
                    Program.Print("Regenerated C* code:", ConsoleColor.Magenta);
                    Program.Print(treelines, true);
                    Program.Print("Expression table:", ConsoleColor.Magenta);
                    Program.Print(from _ in sares.ExpressionTypes select $"{_.Key} ==> {_.Value}");
                    Program.Print("Symbol table:", ConsoleColor.Magenta);
                    Program.Print(from _ in sares.SymbolTable select $"{_.Key} ==> {_.Value}");
                    Program.Print("Global variables:", ConsoleColor.Magenta);
                    Program.Print(from _ in res.Globals from l in _.ToString().Split('\n') select l);
                    Program.Print("Functions:", ConsoleColor.Magenta);
                    Program.Print(from _ in res.Methods from l in _.ToString().Split('\n') select l);
                    Program.Print("Pre-generated assembly instructions:", ConsoleColor.Magenta);
                    Program.Print(from _ in asm where !_.IsComment select _.ToString(), true);
                });

                return new CompilerResult(acode.Split('\n')) { Functions = funcs };
            }
#if !FAIL_HARD
            catch (Exception ex)
            {
                return ex;
            }
#endif
        }
    }

    public struct CompilerResult
    {
        public List<Action> Functions { set; get; }
        public bool Success { set; get; }
        public string[] Lines { set; get; }
        public string Error { set; get; }
        // TODO

        public CompilerResult(params string[] lines)
            : this()
        {
            Error = null;
            Lines = lines;
            Success = true;
        }

        public static implicit operator CompilerResult(Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            while (ex != null)
            {
                sb.Insert(0, ex.StackTrace + '\n');
                sb.Insert(0, ex.Message + '\n');

                ex = ex.InnerException;
            }

            return new CompilerResult
            {
                Success = false,
                Error = sb.ToString(),
            };
        }
    }
}
