using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace MicroWin.functions
{
    internal class MountImage
    {
        public void SetupDirectory()
        {
            string mountPath = Path.Combine(Path.GetTempPath(), "microwin/mount");
            Directory.CreateDirectory(mountPath);
            string scratchPath = Path.Combine(Path.GetTempPath(), "microwin/scratch");
            Directory.CreateDirectory(scratchPath);
        }

        public void Setup(string isoPath)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-Command \"Mount-DiskImage -ImagePath '{isoPath}' -StorageType ISO -PassThru | Get-Volume}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
        }
    }
}
