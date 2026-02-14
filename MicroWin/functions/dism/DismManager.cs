using Microsoft.Dism;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MicroWin.functions.dism
{
    public static class DismManager
    {

        public static Dictionary<int, string> GetWimVersions(string wimPath)
        {
            Dictionary<int, string> versions = new Dictionary<int, string>();

            DismImageInfoCollection imageInfoCollection = GetImageInformation(wimPath);
            if (imageInfoCollection is null)
                return versions;

            foreach (DismImageInfo imageInfo in imageInfoCollection)
            {
                versions.Add(imageInfo.ImageIndex, imageInfo.ImageName);
            }

            return versions;
        }

        public static void MountImage(string wimPath, int index, string mountPath, Action<int> progress)
        {
            // Check whether the file exists, then the index, then the mount path.

            if (!File.Exists(wimPath))
                return;

            DismImageInfoCollection imageInfo = GetImageInformation(wimPath);
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
                File.SetAttributes(wimPath, (File.GetAttributes(wimPath) & ~FileAttributes.ReadOnly));
            }

            try
            {
                DismApi.Initialize(DismLogLevel.LogErrors);
                DismApi.MountImage(wimPath, mountPath, index, false, DismMountImageOptions.None, (currentProgress) =>
                {
                    progress(currentProgress.Current);
                });
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
                    // ignore
                    throw;
                }
            }
        }

        private static DismMountedImageInfoCollection GetMountedImages()
        {
            DismMountedImageInfoCollection mountedImages = null;

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

        private static DismImageInfoCollection GetImageInformation(string wimFile)
        {
            DismImageInfoCollection imageInfo = null;

            try
            {
                DismApi.Initialize(DismLogLevel.LogErrors);
                imageInfo = DismApi.GetImageInfo(wimFile);
            }
            catch (Exception)
            {
                // log or do something
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

        public static void UnmountAndSave(string mountPath, Action<int> progress)
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
            DismMountedImageInfoCollection mountedImages = GetMountedImages();
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
    }
}