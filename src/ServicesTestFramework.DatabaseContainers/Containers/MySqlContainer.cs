using System;
using System.Data.Common;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Configurations.Abstractions;
using DotNet.Testcontainers.Containers.Modules.Abstractions;
using DotNet.Testcontainers.Containers.Modules.Databases;
using MySqlConnector;
using static ServicesTestFramework.DatabaseContainers.Helpers.FileSystemHelper;

namespace ServicesTestFramework.DatabaseContainers.Containers
{
    public class MySqlContainer
    {
        public TestcontainerDatabase Container { get; init; }
        public DbConnection Connection { get; private set; }

        public string MountSourceFolder { get; init; }
        public int HostPort { get; private set; }

        private MySqlContainer()
        { }

        public static MySqlContainer InitializeContainer(TestcontainerDatabaseConfiguration containerConfiguration, string mountSourceFolder, string containerName)
        {
            var container = new TestcontainersBuilder<MySqlTestcontainer>()
                .WithDatabase(containerConfiguration)
                .WithName(containerName)
                .WithMount(mountSourceFolder, "/var/lib/mysql")
                .WithEntrypoint("docker-entrypoint.sh", "--lower-case-table-names=1", "--innodb-page-size=65536", "--innodb-strict-mode=OFF", "--sql-mode=NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION")
                .Build();

            return new MySqlContainer { Container = container, MountSourceFolder = mountSourceFolder };
        }

        public async Task StartContainer()
        {
            if (IsRunning)
                throw new InvalidOperationException("Container already running");

            await Container.StartAsync().ConfigureAwait(false);
            Connection = new MySqlConnection($"{Container.ConnectionString}allowUserVariables=true;");
            HostPort = Container.Port;

            DatabaseContainerPool.Containers.Add(this);
        }

        public async Task StopContainer(bool withCleanUp = true)
        {
            if (!IsRunning)
                throw new InvalidOperationException("Container is not running");

            Connection.Dispose();
            await Container.DisposeAsync().ConfigureAwait(false);

            DatabaseContainerPool.Containers.Remove(this);

            if (withCleanUp)
                CleanupFolder(MountSourceFolder);
        }

        public bool IsRunning => Connection is not null;
    }
}
