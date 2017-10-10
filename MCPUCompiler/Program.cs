using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MCPUCompiler
{
    using static ConsoleColor;
    using static Console;

    public static class Program
    {
        public static readonly string AssemblyName = new FileInfo(typeof(Program).Assembly.Location).Name;


        public static void Main(string[] args)
        {
            $@"
--------------------------------------------
        C* to MCPU Assembly compiler

    Copyright (c) Unknown6656, 2017-{DateTime.Now.Year}
--------------------------------------------
".PrintT(Yellow);

            if (!args.Any())
                PrintUsage();
            else
            {

            }
        }

        private static void PrintUsage() =>
            $@"
Usage:
    {AssemblyName} options
Options:
    -in=...     Input code file
    -jar=...    The MCPUPlugin .jar-file
    -out=...    The output .asm file
".Print();

        private static void Print(this string s, ConsoleColor c = White)
        {
            ForegroundColor = c;
            WriteLine(s);
        }

        private static void PrintT(this string s, ConsoleColor c = White) => s.Trim().Print(c);
    }
}
