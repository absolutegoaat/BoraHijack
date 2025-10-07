using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace BoraHijack.Common
{
    class DisplayOff
    {
        // Import the required Windows API methods
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private const uint WM_SYSCOMMAND = 0x112;
        private const uint SC_MONITORPOWER = 0xF170;
        private const uint MONITOR_OFF = 2;

        public static void Main2()
        {
            IntPtr hwnd = GetForegroundWindow();

            // Post the message to turn off the monitor
            PostMessage(hwnd, WM_SYSCOMMAND, (IntPtr)SC_MONITORPOWER, (IntPtr)MONITOR_OFF);
        }
    }
}
