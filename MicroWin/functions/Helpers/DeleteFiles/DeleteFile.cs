using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using MicroWin.functions.dism;

namespace MicroWin.functions.Helpers.DeleteFile
{
    public static class DeleteFiles
    {
        public static void SafeDeleteDirectory(string path)
        {
            if (!Directory.Exists(path)) return;

            DirectoryInfo directory = new(path);

            if (Directory.Exists(AppState.ScratchPath))
            {
                DismManager.UnmountAndDiscard(AppState.ScratchPath);
            }

            foreach (FileInfo file in directory.GetFiles("*", SearchOption.AllDirectories))
            {
                file.Attributes = FileAttributes.Normal;
            }

            foreach (DirectoryInfo dir in directory.GetDirectories("*", SearchOption.AllDirectories))
            {
                dir.Attributes = FileAttributes.Normal;
            }

            directory.Delete(true);
        }
    }
}
