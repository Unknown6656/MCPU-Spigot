// #define FAIL_HARD

using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System;

using MCPUCompiler.Core.Testing;
using MCPUCompiler.Core;

using static MCPUCompiler.Core.Compiler;

namespace MCPUCompiler
{
    public sealed class Compiler
        : IDisposable
    {
        public (string, int?)[] InstructionSet { get; }
        public ASMCommentLevel CommentingLevel { get; }


        public Compiler((string, int?)[] instructions, ASMCommentLevel lvl = ASMCommentLevel.NoComments)
        {
            InstructionSet = instructions;
            CommentingLevel = lvl;
        }

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
                var tree = Lexer.Parse(code);
                var sares = Parser.Analyze(tree);
                var compl = new ASMBuilder(sares);
                var res = compl.BuildClass(tree);
                var emt = new Emitter(res);
                var asm = emt.Merge();
                var outp = emt.Generate(CommentingLevel);
                var acode = string.Join("\n", outp).Split('\n'); // join is needed as some 'lines' can have line-breaks in themselves
                int linenr = 1;

                foreach (string line in acode)
                {
                    if (!line.Trim().StartsWith(";") && !line.Contains(':'))
                    {
                        string instr = line.Trim() + ' ';

                        if (!InstructionSet.Any(i => i.Item1 == instr))
                            throw Errors.CannotEmitInstruction(instr);
                    }

                    ++linenr;
                }

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

                return new CompilerResult(acode) { Functions = funcs };
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
#if DEBUG
            StringBuilder sb = new StringBuilder();

            while (ex != null)
            {
                sb.Insert(0, $"{ex.GetType()}: {Regex.Replace(ex.Message, @"[ \r\n\t]+", " ")}\n{ex.StackTrace}\n");

                ex = ex.InnerException;
            }

            return new CompilerResult
            {
                Success = false,
                Error = sb.ToString(),
            };
#else
            return new CompilerResult
            {
                Success = false,
                Error = ex.Message,
            };
#endif
        }
    }
}
