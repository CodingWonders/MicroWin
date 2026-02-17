using Microsoft.Dism;
using MicroWin.functions.Helpers.Loggers;
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
            get;
            protected set;
        } = [
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

        public override void RunTask(Action<int> pbReporter, Action<string> curOpReporter)
        {
            RemoveUnwantedPackages(pbReporter, curOpReporter);
        }

        private void RemoveUnwantedPackages(Action<int> pbReporter, Action<string> curOpReporter)
        {
            curOpReporter.Invoke("Getting image packages...");
            DismPackageCollection allPackages = GetPackageList();

            if (allPackages is null) return;

            curOpReporter.Invoke("Filtering image packages...");
            IEnumerable<string> packagesToRemove = allPackages.Select(pkg => pkg.PackageName).Where(pkg =>
                !excludedItems.Any(entry => pkg.IndexOf(entry, StringComparison.OrdinalIgnoreCase) >= 0));

            try
            {
                DismApi.Initialize(DismLogLevel.LogErrors);
                using DismSession session = DismApi.OpenOfflineSession(AppState.ScratchPath);
                int idx = 0;
                foreach (string packageToRemove in packagesToRemove)
                {
                    curOpReporter.Invoke($"Removing package {packageToRemove}...");
                    pbReporter.Invoke((idx / packagesToRemove.Count()) * 100);
                    // we have this because the API throws an exception on removal error
                    try
                    {
                        DismApi.RemovePackageByName(session, packageToRemove);
                    }
                    catch (Exception ex)
                    {
                        DynaLog.logMessage($"ERROR: Failed to remove {packageToRemove}: {ex.Message}");
                    }
                    idx++;
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
