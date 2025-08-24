using System.IO.Compression;

namespace ServicesTestFramework.DatabaseContainers;

internal static class Archiver
{
    public static void Unzip(string dataFilePath, string extractPath)
    {
        DeleteIfExist(extractPath);
        ZipFile.ExtractToDirectory(dataFilePath, extractPath);
    }

    private static void DeleteIfExist(string extractPath)
    {
        if (Directory.Exists(extractPath))
            Directory.Delete(extractPath, true);
    }
}
