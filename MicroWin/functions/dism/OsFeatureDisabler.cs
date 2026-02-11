using Microsoft.Dism;
using MicroWin.functions.Helpers.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroWin.functions.dism
{
    public class OsFeatureDisabler : ImageModificationTask
    {
        public override List<string> excludedItems { 
            get => base.excludedItems; 
            set => base.excludedItems = [
                "Defender",
                "Printing",
                "TelnetClient",
                "PowerShell",
                "NetFx",
                "Media",
                "NFS",
                "SearchEngine",
                "RemoteDesktop"
            ]; 
        }

        public override void RunTask()
        {
            DisableFeatures();
        }

        private void DisableFeatures()
        {
            DismFeatureCollection allFeatures = GetFeatureList();

            IEnumerable<string> featuresToDisable = allFeatures
                .Where(feature => ! new DismPackageFeatureState[3] { DismPackageFeatureState.NotPresent, DismPackageFeatureState.UninstallPending, DismPackageFeatureState.Staged }.Contains(feature.State))
                .Select(feature => feature.FeatureName)
                .Where(feature => !excludedItems.Any(entry => feature.IndexOf(entry, StringComparison.OrdinalIgnoreCase) >= 0));

            try
            {
                DismApi.Initialize(DismLogLevel.LogErrors);
                using DismSession session = DismApi.OpenOfflineSession(AppState.MountPath);
                foreach (string featureToDisable in featuresToDisable)
                {
                    try
                    {
                        DismApi.DisableFeature(session, featureToDisable, "", true);
                    }
                    catch (Exception)
                    {
                        DynaLog.logMessage($"ERROR: Failed to disable {featureToDisable}");
                    }
                }
            }
            catch (Exception)
            {
                DynaLog.logMessage("ERROR: Failed to Initialize DISM");
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

        private DismFeatureCollection GetFeatureList()
        {
            DismFeatureCollection featureList = null;

            try
            {
                DismApi.Initialize(DismLogLevel.LogErrors);
                using DismSession session = DismApi.OpenOfflineSession(AppState.ScratchPath);
                featureList = DismApi.GetFeatures(session);
            }
            catch (Exception)
            {
                DynaLog.logMessage("ERROR: Failed to Initialize DISM");
            }
            finally
            {
                try
                {
                    DismApi.Shutdown();
                }
                catch { }
            }

            return featureList;
        }


    }
}
