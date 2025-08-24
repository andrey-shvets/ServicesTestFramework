using System.Data.Common;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using ServicesTestFramework.DatabaseContainers.Docker;
using Xunit;

namespace ServicesTestFramework.DatabaseContainers.Tests.Fixtures;

public class MySqlDatabaseFixture : IAsyncLifetime
{
    private const string DatabaseName = "testdb";
    public const string Username = "testUser";
    public const string DatabasePassword = "123456789";
    private const string SqlScriptsLocation = "Database";

    private const string DefaultScenarioPlaceholder = "First";

    public DbConnection Connection { get; set; }

    public async Task InitializeAsync()
    {
        var environment = ReadTestEnvironmentConfig();

        if (environment.RestartDatabaseContainer)
            Connection = await CreateContainer(environment);
        else
            Connection = ConnectToContainer(environment) ?? await CreateContainer(environment);

        ApplyMigrations(Connection);
    }

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

    public Task DisposeAsync() => Task.CompletedTask;
}
