using System;
using System.Linq;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using Microsoft.Win32;
#pragma warning disable CS0028

public class DesktopImage
{
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

    const int SPI_SETDESKWALLPAPER = 0x0014;
    const int SPIF_UPDATEINIFILE = 0x01;
    const int SPIF_SENDWININICHANGE = 0x02;

    public static async System.Threading.Tasks.Task MainDSKW(string imageUrl)
    {
         
        string localPath = Path.Combine(Path.GetTempPath(), "wallpaper.jpg");

        using (HttpClient client = new HttpClient())
        {
            byte[] imageBytes = await client.GetByteArrayAsync(imageUrl);
            File.WriteAllBytes(localPath, imageBytes);
        }

        RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
        key.SetValue("WallpaperStyle", "2"); // 2 = stretch
        key.SetValue("TileWallpaper", "0");

        // set wall paper
        bool result = SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, localPath,
            SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);

        if (!result)
        {
            // nothing
        }
    }
}
