using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using static ServicesTestFramework.DatabaseContainers.Tests.Helpers.TestContainerHelpers;

namespace ServicesTestFramework.DatabaseContainers.Tests.MigrationTests
{
    public class MigrationPlaceholdersTests : IAsyncLifetime
    {
        private DatabaseContainer TestContainer { get; set; }

        public Task InitializeAsync() => Task.CompletedTask;

        public async Task DisposeAsync() => await TestContainer.StopContainer();

        [Fact]
        public async Task ApplyMigrations_StartsDatabaseInContainer_WithoutSpecifiedPlaceholders()
        {
            TestContainer = await StartDatabaseInContainer(placeholders: null);
            var databaseClient = StartWebAppWithClient(TestContainer.Connection.ConnectionString);

            var firstCount = await databaseClient.GetFirstTableCount();
            var secondCount = await databaseClient.GetSecondTableCount();
            var thirdCount = await databaseClient.GetThirdTableCount();
            var hotfixCount = await databaseClient.GetHotfixTableCount();

            firstCount.Should().Be(5);
            secondCount.Should().Be(4);
            thirdCount.Should().Be(6);
            hotfixCount.Should().Be(0);
        }

        [Fact]
        public async Task ApplyMigrations_StartsDatabaseInContainer_UsesSpecifiedPlaceholders()
        {
            var placeholders = new Dictionary<string, string> { ["${Scenario}"] = "AdditionalData" };
            TestContainer = await StartDatabaseInContainer(placeholders);
            var databaseClient = StartWebAppWithClient(TestContainer.Connection.ConnectionString);

            var firstCount = await databaseClient.GetFirstTableCount();
            var secondCount = await databaseClient.GetSecondTableCount();
            var thirdCount = await databaseClient.GetThirdTableCount();
            var hotfixCount = await databaseClient.GetHotfixTableCount();

            firstCount.Should().Be(7);
            secondCount.Should().Be(9);
            thirdCount.Should().Be(10);
            hotfixCount.Should().Be(0);
        }

        private static async Task<DatabaseContainer> StartDatabaseInContainer(Dictionary<string, string> placeholders)
        {
            var databaseName = "testdb";
            var userName = "testUser";
            var password = "123456789";
            var sqlScriptsLocation = "Database";

            var containerBuilder = new MySqlContainerBuilder()
                    .SetDatabaseConfiguration(databaseName, userName, password);

            var testContainer = await containerBuilder.StartContainer();

            var migrationLocation = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, sqlScriptsLocation));

            testContainer.Connection.ApplyMigrations(placeholders ?? new Dictionary<string, string>(), migrationLocation);

            return testContainer;
        }
    }
}
