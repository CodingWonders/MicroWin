using Microsoft.Dism;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MicroWin.functions.dism
{
    public class DismManager
    {
        // Fix for "No overload for method 'RunDism' takes 2 arguments"
        public void RunDism(string args, Action<int> progressCallback)
        {
            RunDismWithProgress(args, progressCallback);
        }

        // Standard silent runner
        public void RunDism(string args)
        {
            RunDismWithProgress(args, (p) => { });
        }

        public void RunDismWithProgress(string args, Action<int> progressCallback)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "dism.exe",
                Arguments = args,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = psi })
            {
                process.OutputDataReceived += (sender, e) =>
                {
                    if (string.IsNullOrEmpty(e.Data)) return;

                    // Parsing DISM's [==== 20.0% ====] output
                    if (e.Data.Contains("%"))
                    {
                        try
                        {
                            string percentagePart = e.Data.Split('%')[0];
                            int lastSpace = percentagePart.LastIndexOf(' ');
                            if (lastSpace != -1)
                            {
                                string val = percentagePart.Substring(lastSpace).Trim();
                                if (float.TryParse(val, out float percent))
                                    progressCallback?.Invoke((int)percent);
                            }
                        }
                        catch { /* Ignore parsing errors */ }
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();
            }
        }

        public List<string> GetWimVersions(string wimPath)
        {
            var versions = new List<string>();
            var psi = new ProcessStartInfo
            {
                FileName = "dism.exe",
                Arguments = $"/Get-WimInfo /WimFile:\"{wimPath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(psi))
            {
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                foreach (var line in output.Split('\n'))
                {
                    if (line.Contains("Name :"))
                        versions.Add(line.Replace("Name :", "").Trim());
                }
            }
            return versions;
        }

        public void MountImage(string wimPath, string index, string mountPath, Action<int> progress)
        {
            RunDismWithProgress($"/Mount-Image /ImageFile:\"{wimPath}\" /Index:{index} /MountDir:\"{mountPath}\"", progress);
        }

        private DismMountedImageInfoCollection GetMountedImages()
        {
            DismMountedImageInfoCollection mountedImages = null;

            try
            {
                DismApi.Initialize(DismLogLevel.LogErrors);
                mountedImages = DismApi.GetMountedImages();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                try
                {
                    DismApi.Shutdown();
                }
                catch
                {

                }
            }

            return mountedImages;
        }

        public void UnmountAndSave(string mountPath, Action<int> progress)
        {
            if (!Directory.Exists(mountPath))
            {
                // TODO log this; we immediately return if it doesn't exist.
                return;
            }

            // To be sure, we'll check the mounted images for this one.
            DismMountedImageInfoCollection mountedImages = GetMountedImages();
            if ((mountedImages is null) || (!mountedImages.Any(image => image.MountPath == mountPath)))
            {
                return;
            }

            try
            {
                DismApi.Initialize(DismLogLevel.LogErrors);

                DismProgressCallback progressCallback = (currentProgress, userData) =>
                {
                    int scaledProgress = (currentProgress/ 20)
                    progress(scaledProgress)
                }

                DismApi.UnmountImage(mountPath, true, progressCallback);
            }
            catch (Exception ex)
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

            RunDismWithProgress($"/Unmount-Image /MountDir:\"{mountPath}\" /Commit", progress);
        }

        public void InjectShortcuts(string mountPath)
        {
            string publicDesktop = Path.Combine(mountPath, "Users", "Public", "Desktop");
            if (!Directory.Exists(publicDesktop)) Directory.CreateDirectory(publicDesktop);
    
        }
    }
}