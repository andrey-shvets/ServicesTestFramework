using ServicesTestFramework.DatabaseContainers.Containers;
using static ServicesTestFramework.DatabaseContainers.Helpers.RandomHelper;

namespace ServicesTestFramework.DatabaseContainers;

public static class DatabaseContainerPool
{
    private const string MountSourcePrefix = "mySqlDB";

    public static HashSet<MySqlContainer> Containers { get; } = new HashSet<MySqlContainer>();

    public static int RandomPort(int minValue = 5000)
    {
        for (; ; )
        {
            var port = RandomNumber(minValue);

            if (Containers.Any(c => c.HostPort == port))
                continue;

            return port;
        }
    }

    public static string RandomMountSourceFolder(string prefix = MountSourcePrefix)
    {
        for (; ; )
        {
            var folderName = $"{prefix}_{RandomString()}";

            if (DoesFolderHaveBoundContainer(folderName))
                continue;

            return folderName;
        }
    }

    public static bool DoesFolderHaveBoundContainer(string folder)
    {
        return Directory.Exists(folder) && Containers.Any(c => Path.GetFullPath(c.MountSourceFolder) == Path.GetFullPath(folder));
    }
}