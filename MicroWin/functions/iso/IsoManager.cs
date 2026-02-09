using System;
using System.IO;
using System.Management;
using System.Threading;

namespace MicroWin.functions.iso
{
    public class IsoManager
    {
        private const string WmiScope = "root\\Microsoft\\Windows\\Storage";
        
        public char MountAndGetDrive(string isoPath)
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
            char driveLetter = '\0';

            using ManagementObjectSearcher query = new(WmiScope, volumeQuery);
            while (driveLetter == '\0')
            {
                Thread.Sleep(50);
                using ManagementObjectCollection queryCollection = query.Get();
                foreach (ManagementBaseObject item in queryCollection)
                {
                    driveLetter = item["DriveLetter"].ToString()[0];
                }
            }

            return driveLetter;
        }

        private string BuildIsoObjectPath(string isoPath)
        {
            return String.Format("MSFT_DiskImage.ImagePath={0},StorageType=1", $"\"{isoPath.Replace("\\", "\\\\")}\"");
        }

        public void ExtractIso(string driveLetter, string destination, Action<int> progressCallback)
        {
            string source = $"{driveLetter}:\\";
            Console.WriteLine($"[DEBUG] Starting extraction from {source} to {destination}");

            if (!Directory.Exists(destination)) Directory.CreateDirectory(destination);

            var files = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories);
            Console.WriteLine($"[DEBUG] Found {files.Length} files to copy.");

            int copiedFiles = 0;
            foreach (string file in files)
            {
                try
                {
                    string destFile = file.Replace(source, $"{destination}\\");
                    string destDir = Path.GetDirectoryName(destFile);
                    if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);

                    File.Copy(file, destFile, true);
                    copiedFiles++;

                    int percentage = (int)((double)copiedFiles / files.Length * 100);
                    progressCallback?.Invoke(percentage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DEBUG] Failed to copy {file}: {ex.Message}");
                }
            }
            Console.WriteLine("[DEBUG] Extraction complete.");
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