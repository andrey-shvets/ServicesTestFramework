using ServicesTestFramework.DatabaseContainers.Containers;

namespace ServicesTestFramework.DatabaseContainers.Tests.MigrationTests;

public class DatabaseMigrationTests
{
    private const string DatabaseName = "testdb";
    private const string UserName = "testUser";
    private const string Password = "123456789";
    private const string DefaultScenarioPlaceholder = "First";
    private const string SqlScriptsLocation = "Database";

    private MySqlTestContainer TestContainer { get; set; }

    [After(Test)]
    public async Task DisposeAsync() => await TestContainer.StopContainer();

    [Test]
    public async Task ApplyMigrations_AppliesScriptsFromProvidedFoldersInAscendingOrder()
    {
        var containerBuilder = new MySqlContainerBuilder()
            .SetDatabaseConfiguration(DatabaseName, UserName, Password)
            .WithCleanup();

        TestContainer = await containerBuilder.StartContainer();

        var migrationLocation = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, SqlScriptsLocation));
        var placeholders = new Dictionary<string, string> { ["${Scenario}"] = DefaultScenarioPlaceholder };

        var appliedMigrations = TestContainer.Connection.ApplyMigrations(placeholders, migrationLocation);

        appliedMigrations.Should().NotBeNullOrEmpty();
        appliedMigrations.Should().HaveCount(13);
        appliedMigrations.Should().StartWith("v0_0_0_001__firstTable.sql");
        appliedMigrations.Should().EndWith("v2_0_0_0001__init_data.sql");
        appliedMigrations.Should().BeInAscendingOrder();
    }

    [Test]
    public async Task ApplyMigrations_UsingSnapshotWithAllMigrations_DoesNotApplyAnyAdditionalMigrations()
    {
        var containerBuilder = new MySqlContainerBuilder()
            .SetDatabaseConfiguration(DatabaseName, UserName, Password)
            .WithCleanup();

        var snapshotPath = Path.Combine(AppContext.BaseDirectory, SqlScriptsLocation, "data-snapshot-all.zip");
        containerBuilder.SetDatabaseSnapshot(snapshotPath);

        TestContainer = await containerBuilder.StartContainer();

        var migrationLocation = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, SqlScriptsLocation));
        var placeholders = new Dictionary<string, string> { ["${Scenario}"] = DefaultScenarioPlaceholder };

        var appliedMigrations = TestContainer.Connection.ApplyMigrations(placeholders, migrationLocation);

        appliedMigrations.Should().BeEmpty();
    }

    [Test]
    public async Task ApplyMigrations_UsingSnapshot_ApplesOnlyNewMigrations()
    {
        var containerBuilder = new MySqlContainerBuilder()
            .SetDatabaseConfiguration(DatabaseName, UserName, Password)
            .WithCleanup();

        var snapshotPath = Path.Combine(AppContext.BaseDirectory, SqlScriptsLocation, "data-snapshot.zip");
        containerBuilder.SetDatabaseSnapshot(snapshotPath);

        TestContainer = await containerBuilder.StartContainer();

        var migrationLocation = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, SqlScriptsLocation));
        var placeholders = new Dictionary<string, string> { ["${Scenario}"] = DefaultScenarioPlaceholder };

        var appliedMigrations = TestContainer.Connection.ApplyMigrations(placeholders, migrationLocation);

        appliedMigrations.Should().NotBeNullOrEmpty();
        appliedMigrations.Should().HaveCount(3);
        appliedMigrations.Should().StartWith("v1_0_0_0002__hotfix_too_many_tables.sql");
        appliedMigrations.Should().EndWith("v2_0_0_0001__init_data.sql");
        appliedMigrations.Should().BeInAscendingOrder();
    }
}
