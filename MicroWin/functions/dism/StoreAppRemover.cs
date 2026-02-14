using Microsoft.Dism;
using MicroWin.functions.Helpers.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public override void RunTask()
        {
            RemoveStoreApps();
        }

        private void RemoveStoreApps()
        {
            DismAppxPackageCollection allStoreApps = GetStoreAppsList();
            if (allStoreApps is null) return;

            IEnumerable<string> appsToRemove = allStoreApps.Select(appx => appx.PackageName).Where(appx =>
                !excludedItems.Any(entry => appx.IndexOf(entry, StringComparison.OrdinalIgnoreCase) >= 0));

            try
            {
                DismApi.Initialize(DismLogLevel.LogErrors);
                using DismSession session = DismApi.OpenOfflineSession(AppState.ScratchPath);
                foreach (string appToRemove in appsToRemove)
                {
                    try
                    {
                        DismApi.RemoveProvisionedAppxPackage(session, appToRemove);
                    }
                    catch (Exception ex)
                    {
                        DynaLog.logMessage($"ERROR: Failed to remove {appToRemove}: {ex.Message}");
                    }
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
