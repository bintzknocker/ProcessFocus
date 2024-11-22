using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ProcessFocus.Services
{
    public static class WindowHelper
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const int ALT = 0xA4;
        private const int EXTENDEDKEY = 0x1;
        private const int KEYUP = 0x2;
        private const uint Restore = 9;

        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        [DllImport("user32.dll")]
        static extern bool AllowSetForegroundWindow(int procID);

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        public static void bringToFront(IntPtr handle)
        {
            log.Info($"incoming handle - {handle}");

            // Verify that app is a running process.
            if (handle == IntPtr.Zero)
            {
                log.Info($"incoming handle is zero, returning");
                return;
            }


            var foregroundWindow = GetForegroundWindow();
            log.Info($"foregroundWindow - {foregroundWindow}");


            //if already in front, get out
            if (handle == foregroundWindow)
            {
                log.Info($"handle is foreground window, returning");
                return;
            }

            log.Info($" keydown");

            //AllowSetForegroundWindow(handle.ToInt32());
            // Simulate a key press
            keybd_event((byte)ALT, 0x45, EXTENDEDKEY | 0, 0);

            //SetForegroundWindow(mainWindowHandle);

            log.Info($" keyup");
            // Simulate a key release
            keybd_event((byte)ALT, 0x45, EXTENDEDKEY | KEYUP, 0);

            // Make app the foreground application
            SetForegroundWindow(handle);

            log.Info($" after setforegroundwindow");
        }
    }
}
