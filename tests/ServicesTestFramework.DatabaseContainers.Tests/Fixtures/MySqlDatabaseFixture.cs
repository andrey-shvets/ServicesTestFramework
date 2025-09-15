using System.Data.Common;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using ServicesTestFramework.DatabaseContainers.Docker;

namespace ServicesTestFramework.DatabaseContainers.Tests.Fixtures;

public static class MySqlDatabaseFixture
{
    private const string DatabaseName = "testdb";
    public const string Username = "testUser";
    public const string DatabasePassword = "123456789";
    private const string SqlScriptsLocation = "Database";

    private const string DefaultScenarioPlaceholder = "First";

    public static DbConnection Connection { get; private set; }

    [Before(TestSession)]
    public static async Task TestSessionSetup()
    {
        var environment = ReadTestEnvironmentConfig();

        if (environment.RestartDatabaseContainer)
            Connection = await CreateContainer(environment);
        else
            Connection = ConnectToContainer(environment) ?? await CreateContainer(environment);

        ApplyMigrations(Connection);
    }

    [After(TestSession)]
    public static Task TestSessionTeardown() => Task.CompletedTask;

    private static MySqlConnection ConnectToContainer(TestEnvironmentOptions environment)
    {
        var isContainerRunning = DockerTools.ContainerExists(environment.TestContainerName);

        if (!isContainerRunning)
            return null;

        var mysqlPort = DockerTools.GetMysqlPortForContainer(environment.TestContainerName);
        var connectionString = $"Server=localhost;Port={mysqlPort};Database={DatabaseName};Uid={Username};Pwd={DatabasePassword};allowUserVariables=true;";

        return new MySqlConnection(connectionString);
    }

    private static async Task<DbConnection> CreateContainer(TestEnvironmentOptions environment)
    {
        DockerTools.RemoveContainerIfExists(environment.TestContainerName);

        var containerBuilder = new MySqlContainerBuilder()
            .SetContainerName(environment.TestContainerName)
            .SetMountSourceFolder(environment.TestContainerName)
            .SetDatabaseConfiguration(DatabaseName, Username, DatabasePassword)
            .WithMySqlParam("character-set-server", "utf8")
            .WithMySqlParam("collation-server", "utf8_unicode_ci")
            .WithEnvironment("MYSQL_ROOT_PASSWORD", "123456789")
            .WithCleanup(enabled: environment.RestartDatabaseContainer);

        if (!string.IsNullOrEmpty(environment.MysqlImageTagName))
            containerBuilder.SetImageTagName(environment.MysqlImageTagName);

        if (environment.InitializeDatabaseFromSnapshot)
        {
            var snapshotPath = Path.Combine(AppContext.BaseDirectory, SqlScriptsLocation, "data-snapshot-all.zip");
            containerBuilder.SetDatabaseSnapshot(snapshotPath);
        }

        var container = await containerBuilder.StartContainer();

        return container.Connection;
    }

    private static void ApplyMigrations(DbConnection connection)
    {
        var migrationLocation = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, SqlScriptsLocation));
        var placeholders = new Dictionary<string, string> { ["${Scenario}"] = DefaultScenarioPlaceholder };

        connection.ApplyMigrations(placeholders, migrationLocation);
    }

    private static TestEnvironmentOptions ReadTestEnvironmentConfig()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile("appsettings.tests.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        return configuration.GetSection(TestEnvironmentOptions.Environment).Get<TestEnvironmentOptions>() ?? new TestEnvironmentOptions { InitializeDatabaseFromSnapshot = true };
    }
}
