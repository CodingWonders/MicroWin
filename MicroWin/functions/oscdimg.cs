using System.Diagnostics;
using Microsoft.Win32;
using MicroWin.Helpers.RegistryHelpers;

namespace MicroWin.oscdimg
{
    public static class oscdimgCheck
    {
        public string peToolsPath;
        public string adkKitsRoot => GetKitsRoot -wow64environment=false;
        public string adkKitsRoot_WOW64Environ => GetKitsRoot -wow64environment=true;

        public string expectedADKPath => Path.Combine(adkKitsRoot, "Assessment and Deployment Kit");
        public string expectedADKPath_WOW64Environ => Path.Combine(adkKitsRoot_WOW64Environ, "Assessment and Deployment Kit");

        public string oscdimgPath => Path.Combine(AppState.TempRoot, "oscdimg.exe");
        public bool oscdImgFound => Path.Exists(oscdimgPath);
        
        public static void checkoscdImg()
        {
            if (!oscdImgFound & ((TestKitRootPaths -adkKitsRootPath=$"{expectedADKPath}" -adkKitsRootPath_WOW64Environ=$"{expectedADKPath_WOW64Environ}") == true));
            {
                if (expectedADKPath != "Assessment and Deployment Kit") { peToolsPath = expectedADKPath; };
                if ((peToolsPath == "") & (expectedADKPath_WOW64Environ != "Assessment and Deployment Kit")) { peToolsPath = expectedADKPath_WOW64Environ; };
                if (Environment.Is64BitOperatingSystem) {
                    oscdimgPath = "$peToolsPath\\Deployment Tools\\amd64\\Oscdimg\\oscdimg.exe";
                } else {
                    oscdimgPath = "$peToolsPath\\Deployment Tools\\x86\\Oscdimg\\oscdimg.exe";
                }
            }
            startInstall();
        }

        public static void startInstall()
        {
            // Start the ISO building
            oscdimgProc = Process.Start(oscdimgPath, $"-m - o - u2 - udfver102 - bootdata:2#p0,e,b\"{Path.Combine(AppState.MountPath, "boot", "etfsboot.com")}\"#pEF,e,b\"{Path.Combine(AppState.MountPath, "efi", "microsoft", "boot", "efisys.bin")}\" \"{AppState.MountPath}\" \"{AppState.saveISO}\"");
        }

        public static void GetKitsRoot(bool wow64environment)
        {
            string adk10KitsRoot = "";

            // if we set the wow64 bit on and we're on a 32-bit system, then we prematurely return the value
            if (wow64environment && !Environment.Is64BitOperatingSystem) 
            {
                return adk10KitsRoot;
            }

            if (wow64environment) 
            {
                regPath = "HKLM:\\SOFTWARE\\WOW6432Node\\Microsoft\\Windows Kits\\Installed Roots";
            }
            else {
                regPath = "HKLM:\\SOFTWARE\\Microsoft\\Windows Kits\\Installed Roots";
            };

            if (RegistryHelper.RegistryKeyExists(regPath) == false) 
            {
                return adk10KitsRoot;
            }

            try {
                adk10KitsRoot ; // Get-ItemPropertyValue -Path $regPath -Name "KitsRoot10" -ErrorAction Stop
            } catch {
                // Add logging
            } 
        }
    }
}