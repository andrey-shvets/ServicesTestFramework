using DotNet.Testcontainers.Configurations;
using ServicesTestFramework.DatabaseContainers.Containers;
using static ServicesTestFramework.DatabaseContainers.Helpers.FileSystemHelper;

namespace ServicesTestFramework.DatabaseContainers
{
    public class MySqlContainerBuilder
    {
        private MySqlTestcontainerConfiguration ContainerConfiguration { get; set; }
        private string MountSourceFolderName { get; set; }
        private string SnapshotPath { get; set; }
        private string ContainerName { get; set; }
        private string MySqlImageTagName { get; set; }
        private Dictionary<string, string> MysqlEntryPointParams { get; set; } = new Dictionary<string, string>();
        private bool CleanupEnabled { get; set; } = true;

        public MySqlContainerBuilder SetDatabaseConfiguration(string databaseName, string username, string password)
        {
            if (string.IsNullOrEmpty(databaseName))
                throw new ArgumentException("Database name can not be null or empty.", nameof(databaseName));

            ContainerConfiguration = new MySqlTestcontainerConfiguration { Database = databaseName, Username = username, Password = password };

            return this;
        }

        /// <summary>
        /// Binds and mounts the specified host machine volume into the container. If not set generates name like "mySqlDB_[random alphanumeric string]".
        /// </summary>
        public MySqlContainerBuilder SetMountSourceFolder(string sourceFolderName)
        {
            if (string.IsNullOrEmpty(sourceFolderName))
                throw new ArgumentException("Source folder name can not be null or empty.", nameof(sourceFolderName));

            MountSourceFolderName = sourceFolderName;

            return this;
        }

        public MySqlContainerBuilder SetContainerName(string name)
        {
            ContainerName = name;

            return this;
        }

        /// <summary>
        /// Sets image name. E.g "mysql:8.0.18".
        /// </summary>
        public MySqlContainerBuilder SetImageTagName(string imageTagName)
        {
            if (string.IsNullOrWhiteSpace(imageTagName))
                throw new ArgumentException($"{nameof(imageTagName)} can not be null or empty.", nameof(imageTagName));

            MySqlImageTagName = imageTagName;

            return this;
        }

        public MySqlContainerBuilder SetDatabaseSnapshot(string snapshotPath)
        {
            if (!File.Exists(snapshotPath))
                throw new ArgumentException($"Provided snapshot file does not exist. Path: {snapshotPath}", nameof(snapshotPath));

            SnapshotPath = snapshotPath;

            return this;
        }

        public MySqlContainerBuilder WithMySqlParam(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException($"{nameof(key)} can not be null or empty.", nameof(key));

            MysqlEntryPointParams.Add(key, value);

            return this;
        }

        public MySqlContainerBuilder WithMySqlParam(string key) => WithMySqlParam(key, null);

        public MySqlContainerBuilder WithCleanup(bool enabled)
        {
            CleanupEnabled = enabled;

            return this;
        }

        public async Task<MySqlContainer> StartContainer()
        {
            if (ContainerConfiguration is null)
                throw new ArgumentException("Can not start container. Database configuration is not set.");

            var mountSourceFolder = MountSourceFolderName ?? DatabaseContainerPool.RandomMountSourceFolder();
            var containerName = ContainerName ?? mountSourceFolder;

            PrepareMountSourceFolder(mountSourceFolder, SnapshotPath);

            var mySqlContainer = MySqlContainer.InitializeContainer(ContainerConfiguration, mountSourceFolder, containerName, CleanupEnabled, MySqlImageTagName, MysqlEntryPointParams);
            await mySqlContainer.StartContainer();

            return mySqlContainer;
        }

        private static void PrepareMountSourceFolder(string mountSourceFolder, string snapshotPath)
        {
            var mountSourcePath = Path.GetFullPath(mountSourceFolder);

            CleanupFolder(mountSourcePath);
            Directory.CreateDirectory(mountSourcePath);

            if (!string.IsNullOrEmpty(snapshotPath))
                CopySnapshotData(snapshotPath, mountSourcePath);
        }

        private static void CopySnapshotData(string snapshotPath, string mountSourceFolder)
        {
            var extractPath = Path.Combine(AppContext.BaseDirectory, mountSourceFolder);

            Archiver.Unzip(snapshotPath, extractPath);
        }
    }
}
