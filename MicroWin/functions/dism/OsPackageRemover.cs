using Microsoft.Dism;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MicroWin.functions.dism
{
    public class OsPackageRemover : ImageModificationTask
    {
        public override List<string> excludedItems
        {
            get => base.excludedItems; 
            set => base.excludedItems = [
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
                "OpenSSH",
                "PMCPPC"
            ];
        }

        public override void RunTask()
        {
            RemoveUnwantedPackages();
        }

        private void RemoveUnwantedPackages()
        {
            DismPackageCollection allPackages = GetPackageList();

            List<string> selectedNames = AppState.SelectedPackages.ToList();

            IEnumerable<string> packagesToRemove = allPackages.Select(pkg => pkg.PackageName).Where(pkg =>
                !excludedItems.Any(entry => pkg.IndexOf(entry, StringComparison.OrdinalIgnoreCase) >= 0) &&
                !AppState.SelectedPackages.Any(entry => entry.Equals(pkg, StringComparison.OrdinalIgnoreCase)));

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

        private DismPackageCollection GetPackageList()
        {
            DismPackageCollection packages = null;

            try
            {
                DismApi.Initialize(DismLogLevel.LogErrors);
                using DismSession session = DismApi.OpenOfflineSession(AppState.ScratchPath);
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
