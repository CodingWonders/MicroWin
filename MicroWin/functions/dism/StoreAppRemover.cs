using Microsoft.Dism;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MicroWin.functions.dism
{
    public static class StoreAppRemover
    {
        private static List<string> excludedStoreApps = new List<string>
        {
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
        };

        public static void RemoveStoreApps()
        {
            DismAppxPackageCollection allStoreApps = GetStoreAppsList();

            IEnumerable<string> appsToRemove = allStoreApps.Select(appx => appx.PackageName).Where(appx =>
                !excludedStoreApps.Any(entry => appx.IndexOf(entry, StringComparison.OrdinalIgnoreCase) >= 0));

            try
            {
                DismApi.Initialize(DismLogLevel.LogErrors);
                using DismSession session = DismApi.OpenOfflineSession(AppState.MountPath);
                foreach (string appToRemove in appsToRemove)
                {
                    try
                    {
                        DismApi.RemoveProvisionedAppxPackage(session, appToRemove);
                    }
                    catch (Exception)
                    {
                        // TODO log here...
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

        private static DismAppxPackageCollection GetStoreAppsList()
        {
            DismAppxPackageCollection storeApps = null;

            try
            {
                DismApi.Initialize(DismLogLevel.LogErrors);
                using DismSession session = DismApi.OpenOfflineSession(AppState.MountPath);
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
