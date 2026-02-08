using System.Collections.Generic;
using System.IO;

namespace MicroWin
{
    public class UserAccount
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }

    public static class AppState
    {
        // 1. Path Definitions
        public static string IsoPath { get; set; }
        public static string TempRoot => Path.Combine(Path.GetTempPath(), "microwin");

        // These provide the locations for the extraction and DISM workspace
        public static string ExtractPath => $"{Path.Combine(TempRoot, "extracted")}\\";
        public static string MountPath => $"{Path.Combine(TempRoot, "mount")}\\";
        public static string ScratchPath => $"{Path.Combine(TempRoot, "scratch")}\\";

        // 2. Selection Data
        public static int SelectedImageIndex { get; set; }
        public static bool IsAuto { get; set; }

        // 3. Collections
        public static List<UserAccount> UserAccounts { get; set; } = new List<UserAccount>();
        public static List<string> SelectedPackages { get; set; } = new List<string>();

        // 4. Customization Toggles
        public static bool AddWinUtilShortcut { get; set; }
        public static bool AddReportingToolShortcut { get; set; }

        public static string saveISO;

        public static string Version => "V1.0";
    }
}