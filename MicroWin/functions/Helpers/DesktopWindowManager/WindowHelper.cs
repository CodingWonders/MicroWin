using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.Remoting.Messaging;

namespace MicroWin.functions.Helpers.DesktopWindowManager
{
    public class WindowHelper
    {
        public sealed class NativeMethods
        {
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr GetSystemMenu(IntPtr hwnd, bool bRevert);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

            [DllImport("dwmapi.dll")]
            public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
        }

        // User32 constants
        const int SC_CLOSE = 0xF060;
        const long MF_BYCOMMAND = 0;
        const long MF_ENABLED = 0;
        const long MF_GRAYED = 1;
        const long MF_DISABLED = 2;

        // DwmApi constants
        const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        const int WS_EX_COMPOSITED = 0x2000000;
        const int GWL_EXSTYLE = -20;

        public static void DisableCloseCapability(IntPtr wndHandle)
        {
            if (!wndHandle.Equals(IntPtr.Zero))
            {
                IntPtr menu = NativeMethods.GetSystemMenu(wndHandle, false);
                if (!menu.Equals(IntPtr.Zero))
                    NativeMethods.EnableMenuItem(menu, SC_CLOSE, (uint)(MF_BYCOMMAND | MF_GRAYED | MF_DISABLED));
            }
        }

        public static void EnableCloseCapability(IntPtr wndHandle)
        {
            if (!wndHandle.Equals(IntPtr.Zero))
            {
                IntPtr menu = NativeMethods.GetSystemMenu(wndHandle, false);
                if (!menu.Equals(IntPtr.Zero))
                    NativeMethods.EnableMenuItem(menu, SC_CLOSE, (uint)(MF_BYCOMMAND | MF_ENABLED));
            }
        }

        public static IntPtr? GetWindowHandleFromControl(Control ctrl)
        {
            return ctrl?.Handle;
        }

        const int DARKMODE_MINMAJOR = 10;
        const int DARKMODE_MINMINOR = 0;
        const int DARKMODE_MINBUILD = 18362;

        public static void ToggleDarkTitleBar(IntPtr hwnd, bool darkMode)
        {
            int attribute = darkMode ? 1 : 0;
            if (!IsWindowsVersionOrGreater(DARKMODE_MINMAJOR, DARKMODE_MINMINOR, DARKMODE_MINBUILD))
                return;
            int result = NativeMethods.DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref attribute, 4);
        }

        private static bool IsWindowsVersionOrGreater(int majorVersion, int minorVersion, int buildNumber)
        {
            Version version = Environment.OSVersion.Version;
            return version.Major > majorVersion ||
                (version.Major == majorVersion && version.Minor > minorVersion) ||
                (version.Major == majorVersion && version.Minor == minorVersion && version.Build >= buildNumber);
        }

        public static int ScaleLogical(int px)
        {
            Single dx;
            Control control = new();
            Graphics g = control.CreateGraphics();

            try
            {
                dx = g.DpiX;
            }
            finally
            {
                g.Dispose();
            }

            return (int)(px * (dx / 96.0));
        }

        public static Point ScalePositionLogical(int posX, int posY)
        {
            return new Point(ScaleLogical(posX), ScaleLogical(posY));
        }

        public static Size ScaleSizeLogical(int width, int height)
        {
            return new Size(ScaleLogical(width), ScaleLogical(height));
        }

        public static Single GetSystemDpi()
        {
            return (Single)ScaleLogical(100);
        }

        public static void DisplayToolTip(object tooltipSender, string tooltipMessage)
        {
            if (tooltipSender is Control ctrl)
            {
                ToolTip displayedToolTip = new();
                displayedToolTip.SetToolTip(ctrl, tooltipMessage);
            }
        }
    }
}
