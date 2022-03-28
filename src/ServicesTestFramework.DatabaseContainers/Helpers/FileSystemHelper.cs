using System.IO;
using System.Linq;

namespace ServicesTestFramework.DatabaseContainers.Helpers
{
    internal class FileSystemHelper
    {
        public static void CleanupFolder(string path)
        {
            if (!Directory.Exists(path))
                return;

            try
            {
                Directory.Delete(path, recursive: true);
            }
            catch (IOException ex)
            {
                //In some cases recursive delete removes all files/subfolders, but can't delete the root folder.
                if (!Directory.EnumerateFileSystemEntries(path).Any())
                    throw new IOException($"Failed to clean up folder {path}. Some files/subfolders were not deleted.", ex);
            }
        }
    }
}
