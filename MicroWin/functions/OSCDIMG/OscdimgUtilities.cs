using MicroWin.functions.Helpers.Loggers;
using MicroWin.functions.Helpers.RegistryHelpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;

namespace MicroWin.OSCDIMG
{
    public static class OscdimgUtilities
    {
        public static string peToolsPath;
        public static string adkKitsRoot => GetKitsRoot(false);
        public static string adkKitsRoot_WOW64Environ => GetKitsRoot(true);

        public static string expectedADKPath => Path.Combine(adkKitsRoot, "Assessment and Deployment Kit");
        public static string expectedADKPath_WOW64Environ => Path.Combine(adkKitsRoot_WOW64Environ, "Assessment and Deployment Kit");

        public static string oscdimgPath { get; set; } = Path.Combine(AppState.TempRoot, "oscdimg.exe");
        public static bool oscdImgFound => File.Exists(oscdimgPath);
        
        public static void checkoscdImg()
        {
            if (!oscdImgFound && TestKitRootPaths(expectedADKPath, expectedADKPath_WOW64Environ))
            {
                if (expectedADKPath != "Assessment and Deployment Kit") { peToolsPath = expectedADKPath; };
                if ((peToolsPath == "") & (expectedADKPath_WOW64Environ != "Assessment and Deployment Kit")) { peToolsPath = expectedADKPath_WOW64Environ; };
                if (Environment.Is64BitOperatingSystem) {
                    oscdimgPath = $"{peToolsPath}\\Deployment Tools\\amd64\\Oscdimg\\oscdimg.exe";
                } else {
                    oscdimgPath = $"{peToolsPath}\\Deployment Tools\\x86\\Oscdimg\\oscdimg.exe";
                }
            }

            if (!File.Exists(oscdimgPath))
            {
                using (var client = new HttpClient())
                {
                    var data = client.GetByteArrayAsync("https://github.com/CodingWonders/MicroWin/raw/main/MicroWin/tools/oscdimg.exe").GetAwaiter().GetResult();
                    File.WriteAllBytes(oscdimgPath, data);
                }
            }
            startInstall();
        }

        public static void startInstall()
        {
            // Start the ISO building
            Process oscdimgProc = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = oscdimgPath,
                    Arguments = $"-m -o -u2 -udfver102 -bootdata:2#p0,e,b{Path.Combine(AppState.MountPath, "boot", "etfsboot.com")}#pEF,e,b{Path.Combine(AppState.MountPath, "efi", "microsoft", "boot", "efisys.bin")} \"{AppState.MountPath}\" \"{AppState.saveISO}\""
                }
            };
            oscdimgProc.Start();
            oscdimgProc.WaitForExit();
            DynaLog.logMessage($"Process exited with code {oscdimgProc.ExitCode}.");
        }

        public static string GetKitsRoot(bool wow64environment)
        {
            string adk10KitsRoot = string.Empty;
            string regPath;

            // if we set the wow64 bit on and we're on a 32-bit system, then we prematurely return the value
            if (wow64environment && !Environment.Is64BitOperatingSystem) 
            {
                return adk10KitsRoot;
            }

            if (wow64environment) 
            {
                regPath = "SOFTWARE\\WOW6432Node\\Microsoft\\Windows Kits\\Installed Roots";
            }
            else {
                regPath = "SOFTWARE\\Microsoft\\Windows Kits\\Installed Roots";
            };

            if (RegistryHelper.RegistryKeyExists(regPath) == false) 
            {
                return adk10KitsRoot;
            }

            try {
                adk10KitsRoot = RegistryHelper.QueryRegistryValue(regPath, "KitsRoot10").Data.ToString() ; // Get-ItemPropertyValue -Path $regPath -Name "KitsRoot10" -ErrorAction Stop
            } catch {
                // Add logging
            }

            return adk10KitsRoot;
        }

        public static bool TestKitRootPaths(string adkKitsRootPath, string adkKitsRootPath_WOW64Environ)
        {
            if (File.Exists(adkKitsRootPath) ^ File.Exists(adkKitsRootPath_WOW64Environ))
            {
                return true;
            }

            return false;
        }
    }
}