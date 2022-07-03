using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
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
            var entryPointParams = new List<string>
            {
                "docker-entrypoint.sh",
                "--lower-case-table-names=1",
                "--innodb-page-size=65536",
                "--innodb-strict-mode=OFF",
                "--sql-mode=NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION"
            }.ToArray();

            var mountSourcePath = Path.GetFullPath(mountSourceFolder);

            var container = new TestcontainersBuilder<MySqlTestcontainer>()
                .WithDatabase(containerConfiguration)
                .WithName(containerName)
                .WithBindMount(mountSourcePath, "/var/lib/mysql")
                .WithEntrypoint(entryPointParams)
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
