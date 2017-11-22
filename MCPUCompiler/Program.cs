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

using static System.ConsoleColor;
using static System.Console;


namespace MCPUCompiler
{
    public static unsafe class Program
    {
        public static readonly string AssemblyName = new FileInfo(typeof(Program).Assembly.Location).Name;
        public const string OPCODENUM_PATH = "epsilonpotato/mcpu/mcpuarch/MCPUOpcodeNumber";
        public const string OPCODE_PATH = "epsilonpotato/mcpu/mcpuarch/opcodes/";
        public const string ANNOT_PREL = "RuntimeVisibleAnnotations";


        public static void Main(string[] args)
        {
            Dictionary<string, string> dic = CheckRequirements(args);

            if (dic is null)
                return;

            byte[] jar = dic.ContainsKey("jar") ? File.ReadAllBytes(dic["jar"]) : Resources.MCPUPlugin;
            (string, int?)[] instructions = null;

            try
            {
                using (MemoryStream ms = new MemoryStream(jar))
                using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Read, false))
                {
                    instructions = (from entry in zip.Entries
                                    let name = entry.FullName.ToLower().Replace('\\', '/').Trim('/')
                                    where name.StartsWith(OPCODE_PATH)
                                    let opcode = FetchOpcodeNumber(entry)
                                    select (name.Replace(OPCODE_PATH, "").Replace(".class", ""), opcode <= 0xffff ? opcode : null)).ToArray();

                    using (Stream s = zip.GetEntry("plugin.yml").Open())
                    using (StreamReader sr = new StreamReader(s))
                    {
                        Match m = null;
                        var yml = (from ln in sr.ReadToEnd().Split('\r', '\n')
                                   where ln.match(@"^(?<name>[^\:\s]+)\s*\:\s*(?<value>[^\s].*)$", out m)
                                   select new
                                   {
                                       Name = m.Groups["name"].ToString(),
                                       Value = m.Groups["value"].ToString()
                                   }).ToDictionary(x => x.Name, x => x.Value);

                        $"Loaded .jar file:\n\t   name: {yml["name"]}\n\tversion: {yml["version"]}\n\t author: {yml["author"]}\n\t descr.: {yml["description"]}".Print();
                    }
                }
            }
            catch
            {
                "The given .jar file does not seem to contain a valid MCPU Minecraft plugin.".Print(Red);
            }
            
            if (instructions != null)
                using (Compiler cmp = new Compiler(instructions, dic.ContainsKey("nocomments")))
                {
                    "Instructions provided by the .jar file:".Print();

                    Print(from ins in instructions select $"    {(ins.Item2 is null ? "----: " : $"{ins.Item2.Value:x4}: ")}{ins.Item1}");

                    string code = Core.Testing.Tests.Code04;
                    // string code = File.ReadAllText(dic["in"]);
                    CompilerResult result = cmp.Compile(code);

                    "Loaded the following source code:".Print(Cyan);

                    Print(code.Split('\n'), true);
                    Print("");

                    if (result.Success)
                    {
                        foreach (var f in result.Functions ?? new List<Action>())
                            f?.Invoke();

                        "The compilation was successful. Generated instructions:".Print(Green);
                        
                        Print(result.Lines, true);
                        Print("");

                        using (FileStream fs = new FileStream(dic["out"], FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                        using (StreamWriter wr = new StreamWriter(fs))
                            foreach (string line in result.Lines)
                                wr.WriteLine(line);
                    }
                    else
                    {
                        "The compiler could not compile the input file due to the follwing reason(s):".Print(Red);

                        result.Error.Split('\n').Print();
#if DEBUG
                        "".Print();
#endif
                    }
                }

            Quit();
        }

        private static int? FetchOpcodeNumber(ZipArchiveEntry entry)
        {
            try
            {
                int gindex = 0;
                uint opc;

                using (Stream s = entry.Open())
                using (StreamReader sr = new StreamReader(s, Encoding.Default))
                {
                    string raw = sr.ReadToEnd();

                    foreach (string str in new[] { ANNOT_PREL, OPCODENUM_PATH, "value" })
                    {
                        int index = raw.IndexOf(str);

                        index += str.Length;
                        raw = raw.Substring(index);

                        gindex += index;
                    }
                }

                ++gindex;

                using (Stream s = entry.Open())
                {
                    byte[] bytes = new byte[gindex + 4];

                    s.Read(bytes, 0, bytes.Length);

                    fixed (byte* ptr = bytes)
                        opc = *((uint*)(ptr + gindex));
                }

                return (int)(((opc >> 24) & 0xff) |
                            (((opc >> 16) & 0xff) << 8) |
                            (((opc >> 8) & 0xff) << 16) |
                            ((opc & 0xff) << 24));
            }
            catch
            {
                return null;
            }
        }

        private static void Quit()
        {
            if (Debugger.IsAttached)
            {
                "Press any key to exit ...".Print();

                ReadKey(true);
            }
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
                                                  where arg.match(@"(-?-|\/)(?<name>[a-z]\w*)([\=\:](?<value>.*))?", out m)
                                                  select new
                                                  {
                                                      Key = m.Groups["name"].ToString().ToLower(),
                                                      Value = m.Groups["value"]?.ToString() ?? ""
                                                  }).ToDictionary(x => x.Key, x => x.Value);

                if (!dic.ContainsKey("in"))
                    "No input code file was provided.".Print(Red);
                else if (!File.Exists(dic["in"]))
                    "The input code file does not exist or could not be found.".Print(Red);
                else if (!dic.ContainsKey("out"))
                    "No output file was provided.".Print(Red);
                else
                    return dic;
            }

            Quit();

            return null;
        }

        private static void PrintUsage() =>
            $@"
Usage:
    {AssemblyName} options
Options:
    -in=...         Input code file
    -jar=...        The MCPUPlugin .jar-file
    -out=...        The output .asm file
    -no-comments    The output .asm file will not contain any code comments
".Print();

        internal static void Print(this IEnumerable<string> s, bool linenumbers = false, ConsoleColor c = White)
        {
            if (linenumbers)
            {
                int i = 0;

                foreach (string line in s)
                    $"{++i,4}: {line}".Print(c);
            }
            else
                foreach (string line in s)
                    line.Print(c);
        }

        internal static void Print(this string s, ConsoleColor c = White)
        {
            ForegroundColor = DarkGray;
            Write($"[{DateTime.Now:HH:mm:ss.ffffff}] ");
            ForegroundColor = c;
            WriteLine(s);
        }

        internal static void PrintT(this string s, ConsoleColor c = White) => s.Trim().Print(c);

        internal static bool match(this string str, string pat, out Match m, RegexOptions opt = RegexOptions.IgnoreCase | RegexOptions.Compiled) =>
            (m = Regex.Match(str, pat, opt)).Success;

        internal static string[] SpliceText(this string text, string pattern) => Regex.Matches(text + " ", pattern).Cast<Match>().Select(m => m.Value).ToArray();
    }
}
