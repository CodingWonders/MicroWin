using Microsoft.Dism;
using MicroWin.functions.dism;
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
    public static class DriverExportHelper
    {

        private static bool CopyRecursive(string? SourceDirectory, string DestinationDirectory)
        {
            if (!Directory.Exists(SourceDirectory))
                return false;

            if (!Directory.Exists(DestinationDirectory))
            {
                try
                {
                    Directory.CreateDirectory(DestinationDirectory);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            try
            {
                string[] dirsInSource = Directory.GetDirectories(SourceDirectory, "*", SearchOption.AllDirectories);
                foreach (string dirInSource in dirsInSource)
                {
                    string sourcePath = dirInSource.Substring(SourceDirectory.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    string destinationPath = Path.Combine(DestinationDirectory, sourcePath);

                    if (!Directory.Exists(destinationPath))
                        Directory.CreateDirectory(destinationPath);
                }

                foreach (string FileToCopy in Directory.GetFiles(SourceDirectory, "*", SearchOption.AllDirectories))
                {
                    string sourcePath = FileToCopy.Substring(SourceDirectory.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    string destinationPath = Path.Combine(DestinationDirectory, sourcePath);

                    File.Copy(FileToCopy, destinationPath, true);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private static DismDriverPackageCollection? GetOnlineDrivers()
        {
            DismDriverPackageCollection? drivers = null;
            try
            {
                DismApi.Initialize(DismLogLevel.LogErrors);
                using DismSession session = DismApi.OpenOnlineSession();
                drivers = DismApi.GetDrivers(session, false);
            }
            catch (Exception)
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

            return drivers;
        }

        public static bool ExportDrivers(string DestinationDir, Action<string?>? progressReporter = null)
        {
            DynaLog.logMessage("Preparing to export all drivers from system driver store...");
            if (!Directory.Exists(DestinationDir))
            {
                DynaLog.logMessage("Destination directory does not exist. Attempting to create it...");
                try
                {
                    Directory.CreateDirectory(DestinationDir);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return DismManager.RunDismProcess($"/online /English /export-driver /destination=\"{DestinationDir}\"", progressReporter) == 0;
        }

        public static bool ExportDrivers(string DestinationDir, string ClassName, Action<string?>? progressReporter = null)
        {
            DismDriverPackageCollection? onlineDrivers = GetOnlineDrivers();
            if (onlineDrivers is null)
                return false;

            IEnumerable<DismDriverPackage> filteredDrivers = onlineDrivers.Where(driver => driver.ClassName.Equals(ClassName, StringComparison.OrdinalIgnoreCase));
            if (filteredDrivers is null)
                return false;

            if (progressReporter is not null)
                progressReporter.Invoke($"Exporting {filteredDrivers.Count()} drivers...");

            foreach (DismDriverPackage filteredDriver in filteredDrivers)
            {
                string drvName = Path.GetFileName(filteredDriver.OriginalFileName);
                string destinationDriverPath = Path.Combine(DestinationDir, drvName);

                if (progressReporter is not null)
                    progressReporter.Invoke($"Exporting driver file \"{drvName}\"");

                if (CopyRecursive(Path.GetDirectoryName(filteredDriver.OriginalFileName), destinationDriverPath))
                {
                    if (progressReporter is not null)
                        progressReporter.Invoke($"Driver file \"{drvName}\" was successfully exported.");
                }
                else
                {
                    if (progressReporter is not null)
                        progressReporter.Invoke($"Driver file \"{drvName}\" was not exported.");
                }
            }

            return true;
        }

    }
}
