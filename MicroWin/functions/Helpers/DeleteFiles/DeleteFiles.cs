namespace MicroWin.functions.Helpers.DeleteFiles
{
    public static class DeleteFiles
    {
        public static void SafeDeleteDirectory(string path)
        {
            if (!Directory.Exists(path)) return;

            var directory = new DirectoryInfo(path);

            foreach (var file in directory.GetFiles("*", SearchOption.AllDirectories))
            {
                file.Attributes = FileAttributes.Normal;
            }

            foreach (var dir in directory.GetDirectories("*", SearchOption.AllDirectories))
            {
                file.Attributes = FileAttributes.Normal;
            }

            directory.Delete(true);
        }
    }
}
