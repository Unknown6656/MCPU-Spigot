using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System;

using MCPUCompiler.Properties;


namespace MCPUCompiler
{
    using static ConsoleColor;
    using static Console;


    public static class Program
    {
        public static readonly string AssemblyName = new FileInfo(typeof(Program).Assembly.Location).Name;
        public const string OPCODE_PATH = "/com/unknown6656/opcodes";


        public static void Main(string[] args)
        {
            Dictionary<string, string> dic = CheckRequirements(args);

            if (dic is null)
                return;

            byte[] jar = dic.ContainsKey("jar") ? File.ReadAllBytes(dic["jar"]) : Resources.MCPUPlugin;
            string[] instructions;

            using (MemoryStream ms = new MemoryStream(jar))
            using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Read, false))
                instructions = (from entry in zip.Entries
                                where entry.FullName.ToLower().Replace('\\', '/').StartsWith(OPCODE_PATH)
                                select entry.Name).ToArray();
            
        }

        private static Dictionary<string, string> CheckRequirements(string[] args)
        {
            $@"
--------------------------------------------
        C* to MCPU Assembly compiler

    Copyright (c) Unknown6656, 2017-{DateTime.Now.Year}
--------------------------------------------
".Print(Yellow);

            if (!args.Any())
                PrintUsage();
            else
            {
                Match m = null;
                Dictionary<string, string> dic = (from arg in args
                                                  where arg.match(@"(-?-|\/)(?<name>[a-z]\w*)[\=\:](?<value>.*)", out m)
                                                  select new
                                                  {
                                                      Key = m.Groups["name"].ToString().ToLower(),
                                                      Value = m.Groups["value"].ToString()
                                                  }).ToDictionary(x => x.Key, x => x.Value);

                if (!dic.ContainsKey("in"))
                    "No input code file was provided.".Print(Red);
                else if (!File.Exists(dic["in"]))
                    "No input code file does not exist or could not be found.".Print(Red);
                else if (!dic.ContainsKey("out"))
                    "No output file was provided.".Print(Red);
                else
                    return dic;
            }

            if (Debugger.IsAttached)
            {
                "Press any key to exit ...".Print();

                ReadKey(true);
            }

            return null;
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
            ForegroundColor = DarkGray;
            Write($"[{DateTime.Now:HH:mm:ss.ffffff}] ");
            ForegroundColor = c;
            WriteLine(s);
        }

        private static void PrintT(this string s, ConsoleColor c = White) => s.Trim().Print(c);

        internal static bool match(this string str, string pat, out Match m, RegexOptions opt = RegexOptions.IgnoreCase | RegexOptions.Compiled) =>
            (m = Regex.Match(str, pat, opt)).Success;
    }
}
