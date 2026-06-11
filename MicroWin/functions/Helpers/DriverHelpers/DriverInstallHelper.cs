using Microsoft.Dism;
using MicroWin.functions.Helpers.Loggers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroWin.functions.Helpers.DriverHelpers
{
    public static class DriverInstallHelper
    {
        public static void InstallDrivers(string mountPath, string driverSource, Action<string?>? progressReporter = null)
        {
			try
			{
				DynaLog.logMessage("Preparing to import drivers...");
				DismApi.Initialize(DismLogLevel.LogErrors);
				using DismSession session = DismApi.OpenOfflineSession(mountPath);
				IEnumerable<string> infFiles = Directory.EnumerateFiles(driverSource, "*.inf", SearchOption.AllDirectories);
				foreach (string infFile in infFiles)
				{
					try
					{
						DynaLog.logMessage($"Adding driver file \"{Path.GetFileName(infFile)}\"...");
						if (progressReporter is not null)
							progressReporter.Invoke($"Adding driver file \"{Path.GetFileName(infFile)}\"...");
						DismApi.AddDriver(session, infFile, true);
					}
					catch (Exception ex)
					{
                        DynaLog.logMessage($"Could not add driver file \"{Path.GetFileName(infFile)}\": {ex.Message}");
                        if (progressReporter is not null)
                            progressReporter.Invoke($"Could not add driver file \"{Path.GetFileName(infFile)}\"");
                    }
				}
			}
			catch (Exception ex)
			{
                DynaLog.logMessage($"Could not import drivers: {ex.Message}");
                if (progressReporter is not null)
                    progressReporter.Invoke("Could not import drivers.");
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
    }
}
