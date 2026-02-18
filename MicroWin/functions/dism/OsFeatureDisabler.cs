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
            get;
            protected set;
        } = [
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

        public override void RunTask(Action<int> pbReporter, Action<string> curOpReporter, Action<string> logWriter)
        {
            DisableFeatures(pbReporter, curOpReporter, logWriter);
        }

        private void DisableFeatures(Action<int> pbReporter, Action<string> curOpReporter, Action<string> logWriter)
        {
            curOpReporter.Invoke("Getting image features...");
            DismFeatureCollection allFeatures = GetFeatureList();

            if (allFeatures is null) return;

            logWriter.Invoke($"Amount of features in image: {allFeatures.Count}");

            curOpReporter.Invoke("Filtering image features...");
            IEnumerable<string> featuresToDisable = allFeatures
                .Where(feature => ! new DismPackageFeatureState[3] { DismPackageFeatureState.NotPresent, DismPackageFeatureState.UninstallPending, DismPackageFeatureState.Staged }.Contains(feature.State))
                .Select(feature => feature.FeatureName)
                .Where(feature => !excludedItems.Any(entry => feature.IndexOf(entry, StringComparison.OrdinalIgnoreCase) >= 0));

            logWriter.Invoke($"Features to disable: {featuresToDisable.Count()}");

            try
            {
                DismApi.Initialize(DismLogLevel.LogErrors);
                using DismSession session = DismApi.OpenOfflineSession(AppState.ScratchPath);
                int idx = 0;
                foreach (string featureToDisable in featuresToDisable)
                {
                    curOpReporter.Invoke($"Disabling feature {featureToDisable}...");
                    pbReporter.Invoke((int)(((double)idx / featuresToDisable.ToList().Count) * 100));
                    try
                    {
                        DismApi.DisableFeature(session, featureToDisable, null, true);
                    }
                    catch (Exception ex)
                    {
                        logWriter.Invoke($"Feature {featureToDisable} could not be disabled: {ex.Message}");
                        DynaLog.logMessage($"ERROR: Failed to disable {featureToDisable}: {ex.Message}");
                    }
                    idx++;
                }
            }
            catch (Exception)
            {
                DynaLog.logMessage("ERROR: Failed to Initialize DISM");
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
