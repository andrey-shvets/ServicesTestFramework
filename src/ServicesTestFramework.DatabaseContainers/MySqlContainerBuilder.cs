using System;
using System.IO;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Configurations.Abstractions;
using DotNet.Testcontainers.Containers.Configurations.Databases;
using DotNet.Testcontainers.Containers.Modules.Abstractions;
using DotNet.Testcontainers.Containers.Modules.Databases;
using MySqlConnector;
using static ServicesTestFramework.DatabaseContainers.Helpers.FileSystemHelper;
using static ServicesTestFramework.DatabaseContainers.Helpers.RandomHelper;

namespace ServicesTestFramework.DatabaseContainers
{
    public class MySqlContainerBuilder
    {
        private const string MountSourcePrefix = "mySqlData";
        private MySqlTestcontainerConfiguration ContainerConfiguration { get; set; }
        private string MountSourceFolderName { get; set; }
        private string SnapshotPath { get; set; }
        private int HostPort { get; set; } = RandomPort(minValue: 5000);

        public MySqlContainerBuilder SetDatabaseConfiguration(string databaseName, string username, string password)
        {
            if (string.IsNullOrEmpty(databaseName))
                throw new ArgumentException("Database name can not be null or empty.", nameof(databaseName));

            ContainerConfiguration = new MySqlTestcontainerConfiguration { Database = databaseName, Username = username, Password = password };

            return this;
        }

        /// <summary>
        /// Binds and mounts the specified host machine volume into the container. Default value "mySqlData-<hostPort>".
        /// </summary>
        public MySqlContainerBuilder SetMountSourceFolder(string sourceFolderName)
        {
            if (string.IsNullOrEmpty(sourceFolderName))
                throw new ArgumentException("Source folder name can not be null or empty.", nameof(sourceFolderName));

            MountSourceFolderName = sourceFolderName;

            return this;
        }

        public MySqlContainerBuilder SetHostPort(int port)
        {
            if (port <= 0)
                throw new ArgumentException("host port should be positive.", nameof(port));

            HostPort = port;

            return this;
        }

        public MySqlContainerBuilder SetDatabaseSnapshot(string snapshotPath)
        {
            if (!File.Exists(snapshotPath))
                throw new ArgumentException($"Provided snapshot file does not exist. Path: {snapshotPath}", nameof(snapshotPath));

            SnapshotPath = snapshotPath;

            return this;
        }

        public async Task<DatabaseContainer> StartContainer()
        {
            if (ContainerConfiguration is null)
                throw new ArgumentException("Can not start container. Database configuration is not set.");

            MountSourceFolderName ??= $"{MountSourcePrefix}-{HostPort}";

            CleanupMountFolder(MountSourceFolderName);

            if (!string.IsNullOrEmpty(SnapshotPath))
                CopySnapshotData(SnapshotPath, MountSourceFolderName);

            var mySqlContainer = InitializeContainer(ContainerConfiguration, MountSourceFolderName, HostPort);
            var connection = await StartMySqlDatabase(mySqlContainer);

            return new DatabaseContainer { Container = mySqlContainer, Connection = connection };
        }

        private static void CleanupMountFolder(string mountFolderName)
        {
            var mountFolderPath = $"./{mountFolderName}";

            CleanupFolder(mountFolderPath);
            Directory.CreateDirectory(mountFolderPath);
        }

        private static void CopySnapshotData(string snapshotPath, string mountSourceFolder)
        {
            var extractPath = Path.Combine(AppContext.BaseDirectory, mountSourceFolder);

            Archiver.Unzip(snapshotPath, extractPath);
        }

        private static MySqlTestcontainer InitializeContainer(TestcontainerDatabaseConfiguration containerConfiguration, string mountSourceFolderPath, int port)
        {
            return new TestcontainersBuilder<MySqlTestcontainer>()
                .WithDatabase(containerConfiguration)
                .WithMount(mountSourceFolderPath, "/var/lib/mysql")
                .WithEntrypoint("docker-entrypoint.sh", "--lower-case-table-names=1", "--innodb-page-size=65536", "--innodb-strict-mode=OFF", "--sql-mode=NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION")
                .WithPortBinding(port)
                .Build();
        }

        private static async Task<MySqlConnection> StartMySqlDatabase(TestcontainerDatabase container)
        {
            await container.StartAsync().ConfigureAwait(false);
            return new MySqlConnection($"{container.ConnectionString}allowUserVariables=true;");
        }
    }
}
