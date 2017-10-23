using System.Runtime.ConstrainedExecution;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;
using System.Security;
using System.Linq;
using System.Text;
using System.IO;
using System;

namespace ForceOverwrite
{
    using static Program;


    public static unsafe class Program
    {
        public const int RmRebootReasonNone = 0;
        public const int CCH_RM_MAX_APP_NAME = 255;
        public const int CCH_RM_MAX_SVC_NAME = 63;


        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(void* hnd);

        [DllImport("kernel32.dll")]
        public static extern int GetLastError();

        [DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
        public static extern int RmRegisterResources(uint pSessionHandle, uint nFiles, string[] rgsFilenames, uint nApplications, [In] RM_UNIQUE_PROCESS[] rgApplications, uint nServices, string[] rgsServiceNames);

        [DllImport("rstrtmgr.dll", CharSet = CharSet.Auto)]
        public static extern int RmStartSession(out uint pSessionHandle, int dwSessionFlags, string strSessionKey);

        [DllImport("rstrtmgr.dll")]
        public static extern int RmEndSession(uint pSessionHandle);

        [DllImport("rstrtmgr.dll")]
        public static extern int RmGetList(uint dwSessionHandle, out uint pnProcInfoNeeded, ref uint pnProcInfo, [In, Out] RM_PROCESS_INFO[] rgAffectedApps, ref uint lpdwRebootReasons);


        public static int Main(string[] args)
        {
            if (args.Any(s => s.ToLower() == "--delete"))
            {
                Delete(new FileInfo(args[0]));

                return -1;
            }
            
            FileInfo src = new FileInfo(args[0]);
            FileInfo dst = new FileInfo(args[1]);

            if (src.Exists)
                try
                {
                    src.CopyTo(dst.FullName, true);

                    Console.WriteLine($"copied '{src}' ---> '{dst}'");
                }
                catch
                {
                    try
                    {
                        var procs = GetLocking(dst.FullName);

                        Console.WriteLine($"locking process(es): ({procs.Count})\n{string.Join("\n", procs)}");

                        foreach (var proc in procs)
                        {
                            string raw;

                            using (Process hnd = new Process
                            {
                                StartInfo = new ProcessStartInfo
                                {
                                    FileName = "handle.exe",
                                    WorkingDirectory = @"L:\Programs\Sysinternals",
                                    Arguments = $"-nobanner \"{dst.FullName.ToLower()}\"", // -p {proc.Handle} 
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    CreateNoWindow = true
                                }
                            })
                            {
                                hnd.Start();

                                raw = hnd.StandardOutput.ReadToEnd().ToLower();

                                hnd.WaitForExit();
                            }

                            string pat = @"type\:.*\s+(?<fhnd>[0-9a-f]+)\:\s+" + Regex.Escape(dst.FullName.ToLower()).Replace(@"\\", @"(\\|\/)");
                            Match m = Regex.Match(raw, pat, RegexOptions.IgnoreCase);

                            if (uint.TryParse(m.Groups["fhnd"].ToString(), NumberStyles.HexNumber, null, out uint filehnd))
                            {
                                Console.WriteLine($"fetched handle: '{dst.FullName}' --> 0x{filehnd:x8}");
                                Console.WriteLine($"Closing handle .... {(CloseHandle((void*)filehnd) ? "NOPE" : "YARP")}");
                            }
                        }

                        src.CopyTo(dst.FullName, true);

                        Console.WriteLine($"copied '{src}' ---> '{dst}'");
                    }
                    catch (Exception ex)
                    {
                        StringBuilder sb = new StringBuilder();

                        while (ex != null)
                        {
                            sb.Insert(0, ex.StackTrace).Insert(0, ex.Message);

                            ex = ex.InnerException;
                        }

                        Console.WriteLine(ex.ToString());
                    }
                }
            // else Console.Error.WriteLine($"The source file '{src.FullName}' does not exist.");
            
            return -1;
        }
        
        public static void Delete(FileInfo dst)
        {
            if (dst.Exists)
                try
                {
                    dst.Delete();

                    Console.WriteLine($"moved '{dst}' ---> /dev/null");
                }
                catch
                {
                    try
                    {
                        string raw;

                        using (Process hnd = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = "handle.exe",
                                WorkingDirectory = @"L:\Programs\Sysinternals",
                                Arguments = $"-nobanner \"{dst.FullName.ToLower()}\"", // -p {proc.Handle} 
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                CreateNoWindow = true
                            }
                        })
                        {
                            hnd.Start();

                            raw = hnd.StandardOutput.ReadToEnd().ToLower();

                            hnd.WaitForExit();
                        }

                        string pat = @"pid\:\s+(?<pid>[0-9]+)\s+.*type\:.*\s+(?<fhnd>[0-9a-f]+)\:\s+" + Regex.Escape(dst.FullName.ToLower()).Replace(@"\\", @"(\\|\/)");
                        Match m = Regex.Match(raw, pat, RegexOptions.IgnoreCase);

                        if (uint.TryParse(m.Groups["fhnd"].ToString(), NumberStyles.HexNumber, null, out uint filehnd) &&
                            uint.TryParse(m.Groups["pid"].ToString(), out uint pid))
                        {
                            Console.WriteLine($"fetched handle: '{dst.FullName}' --> 0x{filehnd:x8} [0x{pid:x8}]");
                            Console.WriteLine($"Closing handle .... {(CloseHandle((void*)filehnd) ? "NOPE" : "YARP")}");

                            using (Process hnd = new Process
                            {
                                StartInfo = new ProcessStartInfo
                                {
                                    FileName = "handle.exe",
                                    WorkingDirectory = @"L:\Programs\Sysinternals",
                                    Arguments = $"-nobanner -c {filehnd:x} -y -p {pid}",
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    CreateNoWindow = true
                                }
                            })
                            {
                                hnd.Start();

                                Console.WriteLine("$ handle " + hnd.StartInfo.Arguments);
                                Console.WriteLine(hnd.StandardOutput.ReadToEnd().ToLower());

                                hnd.WaitForExit();
                            }
                        }

                        dst.Delete();

                        Console.WriteLine($"moved '{dst.FullName}' ---> /dev/null");
                    }
                    catch (Exception ex)
                    {
                        StringBuilder sb = new StringBuilder();

                        while (ex != null)
                        {
                            sb.Insert(0, ex.StackTrace).Insert(0, ex.Message);

                            ex = ex.InnerException;
                        }

                        Console.Error.WriteLine(sb.ToString());
                    }
                }
                }

        static public List<Process> GetLocking(string path)
        {
            List<Process> processes = new List<Process>();
            string key = Guid.NewGuid().ToString();
            int res;

            if (RmStartSession(out uint handle, 0, key) != 0)
                throw new Exception("Could not begin restart session.  Unable to determine file locker.");
            else
                try
                {
                    const int ERROR_MORE_DATA = 234;
                    uint pnProcInfoNeeded = 0, pnProcInfo = 0, lpdwRebootReasons = RmRebootReasonNone;
                    string[] resources = new string[] { path };
                    
                    if (RmRegisterResources(handle, (uint)resources.Length, resources, 0, null, 0, null) != 0)
                        throw new Exception("Could not register resource.");
                    
                    res = RmGetList(handle, out pnProcInfoNeeded, ref pnProcInfo, null, ref lpdwRebootReasons);

                    if (res == ERROR_MORE_DATA)
                    {
                        RM_PROCESS_INFO[] processInfo = new RM_PROCESS_INFO[pnProcInfoNeeded];

                        pnProcInfo = pnProcInfoNeeded;
                        res = RmGetList(handle, out pnProcInfoNeeded, ref pnProcInfo, processInfo, ref lpdwRebootReasons);

                        if (res == 0)
                        {
                            processes = new List<Process>((int)pnProcInfo);
                            
                            for (int i = 0; i < pnProcInfo; i++)
                                try
                                {
                                    processes.Add(Process.GetProcessById(processInfo[i].Process.dwProcessId));
                                }
                                catch (ArgumentException)
                                {
                                }
                        }
                        else
                            throw new Exception("Could not list processes locking resource.");
                    }
                    else if (res != 0)
                        throw new Exception("Could not list processes locking resource. Failed to get size of result.");
                }
                finally
                {
                    RmEndSession(handle);
                }

            return processes;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RM_UNIQUE_PROCESS
    {
        public int dwProcessId;
        public System.Runtime.InteropServices.ComTypes.FILETIME ProcessStartTime;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct RM_PROCESS_INFO
    {
        public RM_UNIQUE_PROCESS Process;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_APP_NAME + 1)]
        public string strAppName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_SVC_NAME + 1)]
        public string strServiceShortName;
        public RM_APP_TYPE ApplicationType;
        public uint AppStatus;
        public uint TSSessionId;
        [MarshalAs(UnmanagedType.Bool)]
        public bool bRestartable;
    }

    public enum RM_APP_TYPE
    {
        RmUnknownApp = 0,
        RmMainWindow = 1,
        RmOtherWindow = 2,
        RmService = 3,
        RmExplorer = 4,
        RmConsole = 5,
        RmCritical = 1000
    }
}
