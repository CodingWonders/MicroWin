using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Dism;

namespace WinPEInstallerApp.Core
{
    public class InstallerEngine
    {
        // Executes a command and waits for it to finish
        public static void ExecuteCommand(string fileName, string arguments)
        {
            var psi = new ProcessStartInfo(fileName, arguments)
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };
            Process.Start(psi)?.WaitForExit();
        }

        // Creates a temporary DiskPart script to wipe and partition the drive
        public static void PrepareDisk(string diskNumber)
        {
            string scriptPath = Path.Combine(Path.GetTempPath(), "micro_wipe.txt");
            string commands = $@"select disk {diskNumber}
clean
convert gpt
create partition efi size=100
format quick fs=fat32 label=""System""
assign letter=S
create partition msr size=16
create partition primary
format quick fs=ntfs label=""Windows""
assign letter=W
exit";
            File.WriteAllText(scriptPath, commands);
            ExecuteCommand("diskpart.exe", $"/s \"{scriptPath}\"");
        }

        // Applies the WIM using Microsoft.Dism NuGet package
        public static void ApplyImage(string wimPath, IProgress<int> progress)
        {
            // We use the DISM executable directly to ensure compatibility
            // /Apply-Image /ImageFile:D:\sources\install.wim /Index:1 /ApplyDir:W:\
            string args = $"/Apply-Image /ImageFile:\"{wimPath}\" /Index:1 /ApplyDir:W:\\";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dism.exe",
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                }
            };

            process.Start();

            // Basic progress simulation since DISM CLI output parsing is complex
            for (int i = 0; i <= 100; i += 10)
            {
                progress.Report(i);
                System.Threading.Thread.Sleep(500); // Just for UI feel
            }

            process.WaitForExit();
        }

        // Finalizes the bootloader and applies the Product Key
        public static void FinalizeInstall(string productKey)
        {
            // Fixes the UEFI bootloader
            ExecuteCommand("bcdboot.exe", "W:\\Windows /s S: /f UEFI");

            // Injects product key offline if provided
            if (!string.IsNullOrEmpty(productKey))
            {
                ExecuteCommand("dism.exe", $"/Image:W:\\ /Set-ProductKey:{productKey}");
            }
        }
    }
}