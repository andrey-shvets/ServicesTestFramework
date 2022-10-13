using System.IO;
using Ionic.Zip;

namespace ServicesTestFramework.DatabaseContainers;

internal static class Archiver
{
    public static void Unzip(string dataFilePath, string extractPath)
    {
        DeleteIfExist(extractPath);
        using var zip = ZipFile.Read(dataFilePath);
        zip.ExtractAll(extractPath);
    }

    private static void DeleteIfExist(string extractPath)
    {
        if (Directory.Exists(extractPath))
            Directory.Delete(extractPath, true);
    }
}