using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCPUCompiler
{
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

        }
    }

    public struct CompilerResult
    {
        public bool Success { set; get; }

        // TODO
    }
}
