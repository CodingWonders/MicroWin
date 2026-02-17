using MicroWin.Classes;
using System.Collections.Generic;
using System.IO;

namespace MicroWin
{

    public static class AppState
    {
        public static string IsoPath { get; set; }
        public static string TempRoot => Path.Combine(Path.GetTempPath(), "microwin");
        public static string MountPath => $"{Path.Combine(TempRoot, "mount")}";
        public static string ScratchPath => $"{Path.Combine(TempRoot, "scratch")}";
        public static int SelectedImageIndex { get; set; } = 0;
        public static List<UserAccount> UserAccounts { get; set; } = [];
        /// <summary>
        /// Determines whether to encode passwords with Base64
        /// </summary>
        public static bool EncodeWithB64 { get; set; } = true;
        public static bool AddReportingToolShortcut { get; set; }
        public static bool CopyUnattendToFileSystem { get; set; }
        public static DriverExportMode DriverExportMode { get; set; } = DriverExportMode.NoExport;
        public static string SaveISO { get; set; }

        public static string Version => "v1.99.2";
    }
}