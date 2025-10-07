using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Discord.Webhook;

namespace BoraHijack
{
    class ExceptionLog
    {
        private static readonly string LogFilePath = Path.Combine(Path.GetTempPath(), "crashlog.txt");

        public static void Log(string message)
        {
            using (StreamWriter writer = new StreamWriter(LogFilePath, true))
            {
                writer.WriteLine($"[{DateTime.Now}] {message}");
            }
        }

        public static async Task LogException(string context, Exception ex, string webhookUrl = null)  // <-- Use System.Exception here
        {
            using (StreamWriter writer = new StreamWriter(LogFilePath, true))
            {
                writer.WriteLine($"=== {context} ===");
                writer.WriteLine($"Time: {DateTime.Now}");
                writer.WriteLine($"Message:\n{ex.Message}");
                writer.WriteLine($"StackTrace:\n{ex.StackTrace}");
                writer.WriteLine("====================\n");
            }

            if (!string.IsNullOrEmpty(webhookUrl))
            {
                await SendLogToWebhook(webhookUrl, LogFilePath);
            }
        }

        public static async Task SendLogToWebhook(string webhookUrl, string logFilePath)
        {
            if (!File.Exists(logFilePath))
            {
                Console.WriteLine("Log file not found.");
                return;
            }

            string logContent = File.ReadAllText(logFilePath);

            var webhook = new DiscordWebhookClient(webhookUrl);
            var fileAttachment = new Discord.FileAttachment(LogFilePath);

            if (logContent.Length > 1900)
                logContent = logContent.Substring(logContent.Length - 1900);

            try
            {
                await webhook.SendFilesAsync(new[] { fileAttachment }, "Something went wrong", username: "BoraHijack", avatarUrl: "https://files.catbox.moe/zteeqk.png");
                Console.WriteLine("Something went wrong :/");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }
    }
}
