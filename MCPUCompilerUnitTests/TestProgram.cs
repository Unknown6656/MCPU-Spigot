using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;

namespace MCPUCompilerUnitTests
{
    public static class TestProgram
    {
        public static void Main(string[] argv)
        {
            Dictionary<MethodInfo, (string, string)> results = new Dictionary<MethodInfo, (string, string)>();

            foreach (var entry in from type in typeof(TestProgram).Assembly.GetTypes()
                                  let attr = type.GetCustomAttributes(typeof(TestClassAttribute), true)
                                  where attr.Any()
                                  from meth in type.GetMethods()
                                  let mattr = meth.GetCustomAttributes(typeof(TestMethodAttribute), true)
                                  orderby meth.Name ascending
                                  where mattr.Any()
                                  group meth by type into g
                                  let marr = g.ToArray()
                                  select new
                                  {
                                      Class = g.Key,
                                      Methods = marr,
                                      MethodCount = marr.Length,
                                  })
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"----------- {entry.Class.FullName} -----------");

                object instance = Activator.CreateInstance(entry.Class);
                int left = 0;
                int succ = 0;

                foreach (var m in entry.Methods)
                    try
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write($"    [");

                        left = Console.CursorLeft;

                        Console.Write($"    ] {entry.Class.FullName}.{m.Name}");

                        results[m] = ((string, string))m.Invoke(instance, new object[0]);

                        ++succ;

                        Console.CursorLeft = left;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(" OK ");
                    }
                    catch (Exception ex)
                    {
                        StringBuilder sb = new StringBuilder();

                        while (ex != null)
                        {
                            sb.Insert(0, $"{ex.GetType()}: {Regex.Replace(ex.Message, @"[ \r\n\t]+", " ")}\n{ex.StackTrace}\n");

                            ex = ex.InnerException;
                        }

                        Console.CursorLeft = left;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"ERR.\n        {sb.ToString().Replace("\n", "\n" + new string(' ', 8)).TrimEnd()}");
                    }

                int fail = entry.MethodCount - succ;

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\n================ RESULTS ================");
                Console.WriteLine($"        success: {succ,3} (~{succ * 100d / entry.MethodCount:F2} %)");
                Console.WriteLine($"        failure: {fail,3} (~{fail * 100d / entry.MethodCount:F2} %)");
                Console.WriteLine($"        total:   {entry.MethodCount,3}");
            }

            DirectoryInfo resdir = new DirectoryInfo("./results");

            try
            {
                if (resdir.Exists)
                    resdir.Delete(true);

                resdir.Create();
            }
            catch
            {
                // do it again - it sometimes fails due to some stupid win32 explorer bug
                if (resdir.Exists)
                    resdir.Delete(true);

                resdir.Create();
            }

            foreach (var kvp in results)
                if ((kvp.Value.Item1 ?? "").Trim().Length > 0)
                {
                    string name = $"{resdir.FullName}/{kvp.Key.DeclaringType.FullName}.{kvp.Key.Name}";

                    File.WriteAllText(name + ".c", kvp.Value.Item2);
                    File.WriteAllText(name + ".asm", kvp.Value.Item1);
                }

            if (Debugger.IsAttached)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("\nPress any key to exit ...");
                Console.ReadKey(true);
            }
        }
    }
}
