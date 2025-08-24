using System.Collections.Concurrent;
using System.Data.Common;
using MySqlConnector;
using Testcontainers.MySql;
using static ServicesTestFramework.DatabaseContainers.Helpers.FileSystemHelper;

namespace ServicesTestFramework.DatabaseContainers.Containers;

public class MySqlTestContainer
{
    public MySqlContainer Container { get; init; }
    public DbConnection Connection { get; private set; }

    public string MountSourceFolder { get; init; }
    public int HostPort { get; private set; }

    private MySqlTestContainer()
    {
    }

    public static MySqlTestContainer InitializeContainer(
        MySqlConfiguration configuration,
        string mountSourceFolder, string containerName,
        bool cleanupEnabled = true,
        string imageTagName = null,
        IDictionary<string, string> additionalEntryPointParams = null,
        IReadOnlyDictionary<string, string> environmentParams = null)
    {
        var entryPointParams = CombineEntryPointParams(additionalEntryPointParams);
        var mountSourcePath = Path.GetFullPath(mountSourceFolder);
        var containerBuild = new MySqlBuilder()
            .WithDatabase(configuration.Database)
            .WithUsername(configuration.Username)
            .WithPassword(configuration.Password)
            .WithName(containerName)
            .WithBindMount(mountSourcePath, "/var/lib/mysql")
            .WithEntrypoint(entryPointParams)
            .WithCleanUp(cleanupEnabled);

        if (imageTagName is not null)
            containerBuild = containerBuild.WithImage(imageTagName);

        if (environmentParams is not null)
            containerBuild = containerBuild.WithEnvironment(environmentParams);

        var container = containerBuild.Build();

        return new MySqlTestContainer { Container = container, MountSourceFolder = mountSourceFolder };
    }

    public static ConcurrentDictionary<int, int> Steve = new ConcurrentDictionary<int, int>();

    public async Task StartContainer()
    {
        if (IsRunning)
            throw new InvalidOperationException("Container already running");

        await Container.StartAsync().ConfigureAwait(false);
        Connection = new MySqlConnection($"{Container.GetConnectionString()};allowUserVariables=true;");
        HostPort = Container.GetMappedPublicPort(3306);
    }

    public async Task StopContainer(bool withCleanUp = true)
    {
        if (!IsRunning)
            throw new InvalidOperationException("Container is not running");

        Connection.Dispose();
        await Container.DisposeAsync().ConfigureAwait(false);

        if (withCleanUp)
            CleanupFolder(MountSourceFolder);
    }

    public bool IsRunning => Connection is not null;

    private static string[] CombineEntryPointParams(IDictionary<string, string> additionalEntryPointParams)
    {
        var entryPointParams = new Dictionary<string, string>
        {
            { "lower-case-table-names", "1" },
            { "innodb-page-size", "65536" },
            { "innodb-strict-mode", "OFF" },
            { "sql-mode", "NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION" },
            { "innodb_file_per_table", "ON" },
            { "log_bin_trust_function_creators", "ON" }
        };

        if (additionalEntryPointParams is not null)
        {
            foreach (var newParamKey in additionalEntryPointParams.Keys)
                entryPointParams[newParamKey] = additionalEntryPointParams[newParamKey];
        }

        var formattedParams = entryPointParams.Select(p => ToMySqlParam(p.Key, p.Value)).ToList();

        var allParams = new List<string>
        {
            "docker-entrypoint.sh"
        };

        allParams.AddRange(formattedParams);

        return allParams.ToArray();
    }

    private static string ToMySqlParam(string key, string value) => string.IsNullOrEmpty(value) ? $"--{key}" : $"--{key}={value}";
}
