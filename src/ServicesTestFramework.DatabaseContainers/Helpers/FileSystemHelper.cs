namespace ServicesTestFramework.DatabaseContainers.Helpers;

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
            // In some cases recursive delete removes all files/subfolders, but can't delete the root folder.
            if (!Directory.EnumerateFileSystemEntries(path).Any())
                throw new IOException($"Failed to clean up folder {path}. Some files/subfolders were not deleted.", ex);
        }
    }

    public static bool IsEmptyFolder(string path) =>
        !Directory.Exists(path) || Directory.GetFiles(path).Length == 0 && Directory.GetDirectories(path).Length == 0;
}
