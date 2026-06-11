using Microsoft.Dism;
using MicroWin.functions.Helpers.Loggers;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MicroWin.functions.dism
{
    public static class DismManager
    {

        public static int RunDismProcess(string? args, Action<string?>? actionReporter = null)
        {
            Process dismProc = new()
            {
                StartInfo = new()
                {
                    FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "system32", "dism.exe"),
                    Arguments = args
                }
            };

            if (actionReporter is not null)
            {
                dismProc.StartInfo.CreateNoWindow = true;
                dismProc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                dismProc.StartInfo.RedirectStandardOutput = true;
                dismProc.StartInfo.RedirectStandardError = true;
                dismProc.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        actionReporter.Invoke(e.Data);
                };
                dismProc.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        actionReporter.Invoke(e.Data);
                };
            }

            dismProc.Start();
            if (actionReporter is not null)
            {
                dismProc.BeginOutputReadLine();
                dismProc.BeginErrorReadLine();
            }
            dismProc.WaitForExit();
            return dismProc.ExitCode;
        }

        public static void MountImage(string wimPath, int index, string mountPath, Action<int> progress, Action<string> logMessage)
        {
            // Check whether the file exists, then the index, then the mount path.
            logMessage.Invoke($"Preparing to mount image {Path.GetFileName(wimPath)} (index {index})...");
            if (!File.Exists(wimPath))
                return;

            DismImageInfoCollection? imageInfo = GetImageInformation(wimPath);
            if (imageInfo is null || (index < 1 || index > imageInfo.Count))
                return;

            try
            {
                if (!Directory.Exists(mountPath))
                    Directory.CreateDirectory(mountPath);
            }
            catch (Exception)
            {
                // could not create the directory
                return;
            }

            // Check whether the file has readonly privileges; if it has then the API call throws an
            // exception.
            if ((File.GetAttributes(wimPath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                DynaLog.logMessage("Removing readonly...");
                File.SetAttributes(wimPath, (File.GetAttributes(wimPath) & ~FileAttributes.ReadOnly));
            }

            try
            {
                logMessage.Invoke("Beginning mount operation...");
                DismApi.Initialize(DismLogLevel.LogErrors);
                DismApi.MountImage(wimPath, mountPath, index, false, DismMountImageOptions.None, (currentProgress) =>
                {
                    progress(currentProgress.Current);
                });
            }
            catch (Exception ex)
            {
                DynaLog.logMessage($"Image could not be mounted. Message: {ex.Message}");
            }
            finally
            {
                logMessage.Invoke("Finishing mount operation...");
                try
                {
                    DismApi.Shutdown();
                }
                catch
                {
                    // ignore
                    throw;
                }
            }
        }

        private static DismMountedImageInfoCollection? GetMountedImages()
        {
            DismMountedImageInfoCollection? mountedImages = null;

            try
            {
                DismApi.Initialize(DismLogLevel.LogErrors);
                mountedImages = DismApi.GetMountedImages();
            }
            catch (Exception)
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

        public static DismImageInfoCollection? GetImageInformation(string wimFile, Action<Exception?>? exceptionReporter = null)
        {
            DismImageInfoCollection? imageInfo = null;

            try
            {
                DismApi.Initialize(DismLogLevel.LogErrors);
                imageInfo = DismApi.GetImageInfo(wimFile);
            }
            catch (Exception ex)
            {
                DynaLog.logMessage($"Could not get Windows image information: {ex.Message}");
                if (exceptionReporter is not null)
                    exceptionReporter.Invoke(ex);
            }
            finally
            {
                try
                {
                    DismApi.Shutdown();
                } 
                catch
                {
                    // ignore
                }
            }

            return imageInfo;
        }

        public static void UnmountAndSave(string mountPath, Action<int> progress, Action<string> logMessage)
        {
            logMessage.Invoke($"Preparing to unmount image...");
            if (!Directory.Exists(mountPath))
            {
                // TODO log this; we immediately return if it doesn't exist.
                return;
            }

            // To be sure, we'll check the mounted images for this one.
            DismMountedImageInfoCollection? mountedImages = GetMountedImages();
            if ((mountedImages is null) || (!mountedImages.Any(image => image.MountPath == mountPath)))
            {
                return;
            }

            try
            {
                DismApi.Initialize(DismLogLevel.LogErrors);

                logMessage.Invoke($"Saving and unmounting image...");
                DismProgressCallback progressCallback = (currentProgress) =>
                {
                    int scaledProgress = (currentProgress.Current / 2);
                    progress(scaledProgress);
                };

                DismApi.UnmountImage(mountPath, true, progressCallback);
            }
            catch (Exception)
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
        }
        public static void UnmountAndDiscard(string mountPath)
        {
            if (!Directory.Exists(mountPath))
            {
                // TODO log this; we immediately return if it doesn't exist.
                return;
            }

            // To be sure, we'll check the mounted images for this one.
            DismMountedImageInfoCollection? mountedImages = GetMountedImages();
            if ((mountedImages is null) || (!mountedImages.Any(image => image.MountPath == mountPath)))
            {
                return;
            }

            try
            {
                DismApi.Initialize(DismLogLevel.LogErrors);

                DismApi.UnmountImage(mountPath, false);
            }
            catch (Exception)
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
        }

        public static bool ExportImage(string? sourceImage, int? sourceIndex, string? destinationImage, string? compressionType, Action<string?>? actionReporter = null)
        {
            if (!File.Exists(sourceImage))
                return false;

            DismImageInfoCollection? imageInfo = GetImageInformation(sourceImage);
            if (imageInfo is null || (sourceIndex < 1 || sourceIndex > imageInfo.Count))
                return false;

            return RunDismProcess($"/english /export-image /sourceimagefile=\"{sourceImage}\" /sourceindex={sourceIndex} /destinationimagefile=\"{destinationImage}\" /compress={compressionType}", actionReporter) == 0;
        }
    }
}