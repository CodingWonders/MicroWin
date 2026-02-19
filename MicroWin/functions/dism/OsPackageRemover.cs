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

        public override void RunTask(Action<int> pbReporter, Action<string> curOpReporter, Action<string> logWriter)
        {
            RemoveUnwantedPackages(pbReporter, curOpReporter, logWriter);
        }

        private void RemoveUnwantedPackages(Action<int> pbReporter, Action<string> curOpReporter, Action<string> logWriter)
        {
            curOpReporter.Invoke("Getting image packages...");
            DismPackageCollection allPackages = GetPackageList();

            if (allPackages is null) return;

            logWriter.Invoke($"Amount of packages in image: {allPackages.Count}");

            curOpReporter.Invoke("Filtering image packages...");
            IEnumerable<string> packagesToRemove = allPackages.Select(pkg => pkg.PackageName).Where(pkg =>
                !excludedItems.Any(entry => pkg.IndexOf(entry, StringComparison.OrdinalIgnoreCase) >= 0));

            logWriter.Invoke($"Packages to remove: {packagesToRemove.Count()}");

            try
            {
                DismApi.Initialize(DismLogLevel.LogErrors);
                using DismSession session = DismApi.OpenOfflineSession(AppState.ScratchPath);
                int idx = 0;
                foreach (string packageToRemove in packagesToRemove)
                {
                    curOpReporter.Invoke($"Removing package {packageToRemove}...");
                    pbReporter.Invoke((int)(((double)idx / packagesToRemove.ToList().Count) * 100));
                    // we have this because the API throws an exception on removal error
                    try
                    {
                        DismApi.RemovePackageByName(session, packageToRemove);
                    }
                    catch (Exception ex)
                    {
                        logWriter.Invoke($"Package {packageToRemove} could not be removed: {ex.Message}");
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
                pbReporter.Invoke(100);
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
