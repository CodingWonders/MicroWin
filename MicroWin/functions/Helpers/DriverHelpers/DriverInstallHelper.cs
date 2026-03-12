using Microsoft.Dism;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroWin.functions.Helpers.DriverHelpers
{
    public static class DriverInstallHelper
    {
        public static void InstallDrivers(string mountPath, string driverSource)
        {
			try
			{
				DismApi.Initialize(DismLogLevel.LogErrors);
				using DismSession session = DismApi.OpenOfflineSession(mountPath);
				IEnumerable<string> infFiles = Directory.EnumerateFiles(driverSource, "*.inf", SearchOption.AllDirectories);
				foreach (string infFile in infFiles)
				{
					try
					{
						DismApi.AddDriver(session, infFile, true);
					}
					catch (Exception ex)
					{
						// TODO log
					}
				}
			}
			catch (Exception ex)
			{
				// TODO log
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
