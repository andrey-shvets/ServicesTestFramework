using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace ServicesTestFramework.DatabaseContainers.Tests.Fixtures
{
    public class MySqlDatabaseFixture : IAsyncLifetime
    {
        private const string DatabaseName = "testdb";
        private const bool InitializeDatabaseFromSnapshot = true;
        private const string DefaultScenarioPlaceholder = "First";
        private const string SqlScriptsLocation = "Database";
        private static string MountSourceFolder => $"mysqlData{DateTimeOffset.Now.Ticks}";
        private DatabaseContainer Container { get; set; }

        public DbConnection Connection => Container?.Connection;

        public async Task InitializeAsync()
        {
            var containerBuilder = new MySqlContainerBuilder()
                .SetDatabaseConfiguration(DatabaseName, "testUser", "123456789");

            if (InitializeDatabaseFromSnapshot)
            {
                var snapshotPath = Path.Combine(AppContext.BaseDirectory, "Database", "data-snapshot-all.zip");
                containerBuilder.SetDatabaseSnapshot(snapshotPath);
            }

            Container = await containerBuilder.StartContainer();

            var migrationLocation = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, SqlScriptsLocation));
            var placeholders = new Dictionary<string, string> { ["${Scenario}"] = DefaultScenarioPlaceholder };

            Container.Connection.ApplyMigrations(placeholders, migrationLocation);
        }

        public async Task DisposeAsync()
        {
            if(Container is not null)
                await Container.StopContainer();
        }
    }
}
