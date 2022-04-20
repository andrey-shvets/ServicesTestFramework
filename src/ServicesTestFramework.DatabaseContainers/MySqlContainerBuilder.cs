using System;
using System.IO;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers.Configurations.Databases;
using ServicesTestFramework.DatabaseContainers.Containers;
using static ServicesTestFramework.DatabaseContainers.Helpers.FileSystemHelper;

namespace ServicesTestFramework.DatabaseContainers
{
    public class MySqlContainerBuilder
    {
        private MySqlTestcontainerConfiguration ContainerConfiguration { get; set; }
        private string MountSourceFolderName { get; set; }
        private string SnapshotPath { get; set; }

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

        public MySqlContainerBuilder SetDatabaseSnapshot(string snapshotPath)
        {
            if (!File.Exists(snapshotPath))
                throw new ArgumentException($"Provided snapshot file does not exist. Path: {snapshotPath}", nameof(snapshotPath));

            SnapshotPath = snapshotPath;

            return this;
        }

        public async Task<MySqlContainer> StartContainer()
        {
            if (ContainerConfiguration is null)
                throw new ArgumentException("Can not start container. Database configuration is not set.");

            var mountSourceFolder = MountSourceFolderName ?? DatabaseContainerPool.RandomMountSourceFolder();
            var containerName = mountSourceFolder;

            PrepareMountSourceFolder(mountSourceFolder, SnapshotPath);

            var mySqlContainer = MySqlContainer.InitializeContainer(ContainerConfiguration, mountSourceFolder, containerName);
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
