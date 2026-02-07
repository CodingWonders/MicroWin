using Microsoft.Dism;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MicroWin.functions.dism
{
    public class RemovePackages
    {
        private static List<string> excludedPackages = new List<string>
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

        public static void RemoveUnwantedPackages()
        {
            DismPackageCollection allPackages = GetPackageList();

            List<string> selectedNames = AppState.SelectedPackages.ToList();

            IEnumerable<string> packagesToRemove = allPackages.Select(pkg => pkg.PackageName).Where(pkg =>
                !excludedPackages.Any(entry => pkg.IndexOf(entry, StringComparison.OrdinalIgnoreCase) >= 0) &&
                !AppState.SelectedPackages.Any(entry => entry.Equals(pkg, StringComparison.OrdinalIgnoreCase))).ToList();

            try
            {
                DismApi.Initialize(DismLogLevel.LogErrors);
                using DismSession session = DismApi.OpenOfflineSession(AppState.MountPath);
                foreach (string packageToRemove in packagesToRemove)
                {
                    // we have this because the API throws an exception on removal error
                    try
                    {
                        DismApi.RemovePackageByName(session, packageToRemove);
                    }
                    catch (Exception)
                    {
                        // TODO log here...
                    }
                }
            }
            catch (Exception)
            {
                // TODO implement logging here
            }
            finally
            {
                try
                {
                    DismApi.Shutdown();
                }
                catch { }
            }
        }

        private static DismPackageCollection GetPackageList()
        {
            DismPackageCollection packages = null;

            try
            {
                DismApi.Initialize(DismLogLevel.LogErrors);
                using DismSession session = DismApi.OpenOfflineSession(AppState.MountPath);
                packages = DismApi.GetPackages(session);
            }
            catch (Exception)
            {
                // TODO implement the logging
            }
            finally
            {
                try
                {
                    DismApi.Shutdown();
                }
                catch { }
            }

            return packages;
        }
    }
}
