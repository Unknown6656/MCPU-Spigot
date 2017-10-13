using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCPUCompiler
{
    using MCPUCompiler.Core;


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
            try
            {
                var res = Lexer.parser;
                var tree = Lexer.parse(code);

                throw null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }

    public struct CompilerResult
    {
        public bool Success { set; get; }
        public string[] Lines { set; get; }
        public string Error { set; get; }
        // TODO

        public static implicit operator CompilerResult(Exception ex) => new CompilerResult
        {
            Success = false,
            Error = ex.Message,
        };
    }
}
