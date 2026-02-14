using MicroWin.functions.dism;
using MicroWin.functions.Helpers.RegistryHelpers;
using System.IO;

namespace MicroWin.functions.Helpers.DeleteFile
{
    public static class DeleteFiles
    {
        public static void SafeDeleteDirectory(string path)
        {
            if (!Directory.Exists(path)) return;

            if (Directory.Exists(Path.Combine(AppState.ScratchPath, "Windows", "System32", "config")))
            {
                RegistryHelper.UnloadRegistryHive("zSYSTEM");
                RegistryHelper.UnloadRegistryHive("zSOFTWARE");
                RegistryHelper.UnloadRegistryHive("zDEFAULT");
                RegistryHelper.UnloadRegistryHive("zNTUSER");
            }

            var directory = new DirectoryInfo(path);

            if (Directory.Exists(AppState.ScratchPath))
            {
                DismManager.UnmountAndDiscard(AppState.ScratchPath);
            }

            foreach (var file in directory.GetFiles("*", SearchOption.AllDirectories))
            {
                file.Attributes = FileAttributes.Normal;
            }

            foreach (var dir in directory.GetDirectories("*", SearchOption.AllDirectories))
            {
                dir.Attributes = FileAttributes.Normal;
            }

            directory.Delete(true);
        }
    }
}
