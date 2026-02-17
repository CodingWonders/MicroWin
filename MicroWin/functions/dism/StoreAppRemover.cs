using Microsoft.Dism;
using MicroWin.functions.Helpers.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MicroWin.functions.dism
{
    public class StoreAppRemover : ImageModificationTask
    {
        public override List<string> excludedItems { 
            get;
            protected set;
        } = [
                "AppInstaller",
                "Store",
                "Notepad",
                "Printing",
                "YourPhone",
                "Xbox",
                "WindowsTerminal",
                "Calculator",
                "Photos",
                "VCLibs",
                "Paint",
                "Gaming",
                "Extension",
                "SecHealthUI",
                "ScreenSketch",
                "CrossDevice"
            ];

        public override void RunTask(Action<int> pbReporter, Action<string> curOpReporter)
        {
            RemoveStoreApps(pbReporter, curOpReporter);
        }

        private void RemoveStoreApps(Action<int> pbReporter, Action<string> curOpReporter)
        {
            curOpReporter.Invoke("Getting image AppX packages...");
            DismAppxPackageCollection allStoreApps = GetStoreAppsList();
            if (allStoreApps is null) return;

            curOpReporter.Invoke("Filtering image AppX packages...");
            IEnumerable<string> appsToRemove = allStoreApps.Select(appx => appx.PackageName).Where(appx =>
                !excludedItems.Any(entry => appx.IndexOf(entry, StringComparison.OrdinalIgnoreCase) >= 0));

            try
            {
                DismApi.Initialize(DismLogLevel.LogErrors);
                using DismSession session = DismApi.OpenOfflineSession(AppState.ScratchPath);
                int idx = 0;
                foreach (string appToRemove in appsToRemove)
                {
                    curOpReporter.Invoke($"Removing AppX package {appToRemove}...");
                    pbReporter.Invoke((idx / appsToRemove.Count()) * 100);
                    try
                    {
                        DismApi.RemoveProvisionedAppxPackage(session, appToRemove);
                    }
                    catch (Exception ex)
                    {
                        DynaLog.logMessage($"ERROR: Failed to remove {appToRemove}: {ex.Message}");
                    }
                    idx++;
                }
            }
            catch (Exception)
            {
                // log
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

        private DismAppxPackageCollection GetStoreAppsList()
        {
            DismAppxPackageCollection storeApps = null;

            try
            {
                DismApi.Initialize(DismLogLevel.LogErrors);
                using DismSession session = DismApi.OpenOfflineSession(AppState.ScratchPath);
                storeApps = DismApi.GetProvisionedAppxPackages(session);
            }
            catch (Exception)
            {
                // TODO implement logging
            }
            finally
            {
                try
                {
                    DismApi.Shutdown();
                }
                catch { }
            }

            return storeApps;
        }

    }
}
