using System;
using System.IO;
using System.Diagnostics;

namespace MicroWin.functions.iso
{
    public class IsoManager
    {
        public string MountAndGetDrive(string isoPath)
        {
            Console.WriteLine($"[DEBUG] Attempting to mount: {isoPath}");

            // Check if file exists before trying to mount
            if (!File.Exists(isoPath))
            {
                Console.WriteLine("[DEBUG] ERROR: ISO file not found at path.");
                return "";
            }

            string script = $"Mount-DiskImage -ImagePath '{isoPath}' -PassThru | Get-Volume | Select-Object -ExpandProperty DriveLetter";
            string driveLetter = RunPowerShell(script).Trim();

            if (string.IsNullOrEmpty(driveLetter))
                Console.WriteLine("[DEBUG] ERROR: PowerShell returned no drive letter. Check Admin rights.");
            else
                Console.WriteLine($"[DEBUG] ISO mounted successfully to drive: {driveLetter}:");

            return driveLetter;
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
                    string destFile = file.Replace(source, destination);
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
            RunPowerShell($"Dismount-DiskImage -ImagePath '{isoPath}'");
        }

        private void CopyDirectory(string source, string dest)
        {
            foreach (string dir in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dir.Replace(source, dest));

            foreach (string file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
                File.Copy(file, file.Replace(source, dest), true);
        }

        private string RunPowerShell(string command)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-Command \"{command}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using (var process = Process.Start(psi))
            {
                return process.StandardOutput.ReadToEnd();
            }
        }
    }
}