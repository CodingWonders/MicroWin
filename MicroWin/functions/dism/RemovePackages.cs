using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MicroWin.functions.dism
{
    public class RemovePackages
    {
        public static List<string> excludedPackages = new List<string>
        {
            "ApplicationModel",
            "indows-Client-LanguagePack",
            "LanguageFeatures-Basic",
            "Package_for_ServicingStack",
            "DotNet",
            "Notepad",
            "WMIC",
            "Ethernet",
            "Wifi",
            "FodMetadata",
            "Foundation",
            "LanguageFeatures",
            "VBSCRIPT",
            "License",
            "Hello-Face",
            "ISE",
            "OpenSSH"
        };

        // Define the list to hold all features found on the OS
        public static List<string> AllPackages = new List<string>();

        public static void RemoveUnwantedPackages()
        {
            PopulatePackageList();

            var selectedNames = AppState.SelectedPackages.ToList();

            var toRemove = AllPackages.Where(p =>
                !excludedPackages.Any(sub => p.IndexOf(sub, StringComparison.OrdinalIgnoreCase) >= 0) &&
                !AppState.SelectedPackages.Any(s => s.Equals(p, StringComparison.OrdinalIgnoreCase))
            ).ToList();

            foreach (var package in toRemove)
            {
                Process.Start("dism.exe", arguments: $"/English /Image={AppState.MountPath} /RemovePackage=${package}");
            }
        }

        public static void PopulatePackageList()
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "dism.exe",
                Arguments = $"/English /Image={AppState.MountPath} /Get-Packages /Format:Table",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using (Process proc = Process.Start(psi))
            {
                using (StreamReader reader = proc.StandardOutput)
                {
                    string output = reader.ReadToEnd();
                    string[] lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var line in lines)
                    {

                        if (line.Contains("Enabled") || line.Contains("Disabled"))
                        {
                            string featureName = line.Split(' ')[0].Trim();
                            if (!string.IsNullOrEmpty(featureName))
                            {
                                AllPackages.Add(featureName);
                            }
                        }
                    }
                }
                proc.WaitForExit();
            }
        }
    }
}
