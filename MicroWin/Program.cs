using System;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Forms;

namespace MicroWin
{
    static class Program
    {
        [STAThread]
        [SupportedOSPlatform("Windows")]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.Run(new MainForm());
        }
    }
}