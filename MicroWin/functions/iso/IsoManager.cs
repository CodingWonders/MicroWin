using MicroWin.functions.Helpers.Loggers;
using MicroWin.functions.Helpers.WMI;
using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.Versioning;
using System.Threading;

namespace MicroWin.functions.iso
{
    [SupportedOSPlatform("Windows")]
    public class IsoManager
    {
        private const string WmiScope = "root\\Microsoft\\Windows\\Storage";
        
        public char? MountAndGetDrive(string isoPath)
        {
            Console.WriteLine($"[DEBUG] Attempting to mount: {isoPath}");

            // Check if file exists before trying to mount
            if (!File.Exists(isoPath))
            {
                Console.WriteLine("[DEBUG] ERROR: ISO file not found at path.");
                return '\0';
            }

            string isoObjectPath = BuildIsoObjectPath(isoPath);
            using ManagementObject isoObject = new(WmiScope, isoObjectPath, null);
            using ManagementBaseObject inParams = isoObject.GetMethodParameters("Mount");
            isoObject.InvokeMethod("Mount", inParams, null);

            string volumeQuery = String.Format("ASSOCIATORS OF {{{0}}} WHERE ASSOCCLASS = MSFT_DiskImageToVolume RESULTCLASS = MSFT_Volume", isoObjectPath);
            char? driveLetter = '\0';

            using ManagementObjectSearcher query = new(WmiScope, volumeQuery);
            while (driveLetter == '\0')
            {
                Thread.Sleep(50);
                using ManagementObjectCollection queryCollection = query.Get();
                foreach (ManagementBaseObject? item in queryCollection)
                {
                    driveLetter = item["DriveLetter"]?.ToString()?[0];
                }
            }

            return driveLetter;
        }

        private string BuildIsoObjectPath(string isoPath)
        {
            return String.Format("MSFT_DiskImage.ImagePath={0},StorageType=1", $"\"{isoPath.Replace("\\", "\\\\")}\"");
        }

        public void ExtractIso(string? driveLetter, string destination, Action<int> progressCallback, Action<string> fileProgressCallback)
        {
            string source = $"{driveLetter}:\\";
            DynaLog.logMessage($"Starting extraction from {source} to {destination}");

            if (Directory.Exists(destination))
            {
                try
                {
                    Directory.Delete(destination, true);
                }
                catch (Exception)
                {
                    DynaLog.logMessage("Could not delete destination folder... trying alternate method");
                    string cmdProcPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "system32", "cmd.exe"),
                           takeownPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "system32", "takeown.exe"),
                           icaclsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "system32", "icacls.exe");
                    Process cmdRemoverProc = new()
                    {
                        StartInfo = new()
                        {
                            FileName = takeownPath,
                            Arguments = $"/F \"{destination}\" /R /A",
                            UseShellExecute = true,
                            CreateNoWindow = !Debugger.IsAttached,
                            WindowStyle = Debugger.IsAttached ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden
                        }
                    };
                    cmdRemoverProc.Start();
                    cmdRemoverProc.WaitForExit();
                    // since groups in Windows are localized, we need to grab the name of the Administrators group based on its SID
                    ManagementObjectCollection? adminGroupMOC = WMIHelper.GetResultsFromManagementQuery("SELECT * FROM Win32_Group WHERE SID = \"S-1-5-32-544\"");
                    if (adminGroupMOC is not null)
                    {
                        // I enjoy the simplicity of VB in some cases, such as this one. In there, ElementAtOrDefault works without having to cast stuff first...
                        string? adminGroupName = WMIHelper.GetObjectValue(adminGroupMOC.Cast<ManagementObject>().ElementAtOrDefault(0), "Name")?.ToString();
                        if (adminGroupName != "")
                        {
                            cmdRemoverProc.StartInfo.FileName = icaclsPath;
                            cmdRemoverProc.StartInfo.Arguments = $"\"{destination}\" /T /C /grant \"{adminGroupName}:(M)\"";
                            cmdRemoverProc.Start();
                            cmdRemoverProc.WaitForExit();
                        }
                    }
                    try
                    {
                        Directory.Delete(destination, true);
                    }
                    catch
                    {
                        cmdRemoverProc.StartInfo.FileName = cmdProcPath;
                        cmdRemoverProc.StartInfo.Arguments = $"/c rd \"{destination}\" /s /q";
                        cmdRemoverProc.Start();
                        cmdRemoverProc.WaitForExit();
                    }
                }
            }

            if (!Directory.Exists(destination)) Directory.CreateDirectory(destination);

            var files = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories);
            DynaLog.logMessage($"Found {files.Length} files to copy.");

            int copiedFiles = 0;
            foreach (string file in files)
            {
                try
                {
                    string? destFile = file.Replace(source, $"{destination}\\");
                    string? destDir = Path.GetDirectoryName(destFile);
                    if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir ?? "");

                    fileProgressCallback.Invoke(file);

                    File.Copy(file, destFile, true);
                    copiedFiles++;

                    int percentage = (int)((double)copiedFiles / files.Length * 100);
                    progressCallback?.Invoke(percentage);
                }
                catch (Exception ex)
                {
                    DynaLog.logMessage($"Failed to copy {file}: {ex.Message}");
                }
            }
            DynaLog.logMessage("Extraction complete.");
        }

        public void Dismount(string isoPath)
        {
            if (!File.Exists(isoPath))
                return;

            using ManagementObject isoObject = new(WmiScope, BuildIsoObjectPath(isoPath), null);
            using ManagementBaseObject inParams = isoObject.GetMethodParameters("Dismount");
            isoObject.InvokeMethod("Dismount", inParams, null);
        }
    }
}