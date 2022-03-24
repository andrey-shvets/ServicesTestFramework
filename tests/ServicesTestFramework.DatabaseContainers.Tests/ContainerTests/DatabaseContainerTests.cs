using System;
using System.Threading.Tasks;
using FluentAssertions;
using MySqlConnector;
using Xunit;
using static ServicesTestFramework.DatabaseContainers.Tests.Helpers.TestContainerHelpers;

namespace ServicesTestFramework.DatabaseContainers.Tests.ContainerTests
{
    public class DatabaseContainerTests : IAsyncLifetime
    {
        private const string DatabaseName = "testdb";
        private const string UserName = "testUser";
        private const string Password = "123456789";
        private DatabaseContainer TestContainer { get; set; }

        public Task InitializeAsync() => Task.CompletedTask;

        public async Task DisposeAsync() => await TestContainer.StopContainer();

        [Fact]
        public async Task StartContainer_CreatesContainerWithDatabaseInitialized()
        {
            TestContainer = await new MySqlContainerBuilder()
                                .SetDatabaseConfiguration(DatabaseName, UserName, Password)
                                .SetMountSourceFolder(MountSourceFolder)
                                .StartContainer();

            var connectionString = TestContainer.Connection.ConnectionString;

            connectionString.Should().Contain($"Database={DatabaseName}");
            connectionString.Should().Contain($"Uid={UserName}");
            connectionString.Should().Contain($"Pwd={Password}");
        }

        [Fact]
        public async Task StopContainer_DisposesOfContainerAndAllowsToCreateNewContainerRunningFromTheSameMountFolder()
        {
            var containerBuilder = new MySqlContainerBuilder()
                .SetDatabaseConfiguration(DatabaseName, UserName, Password)
                .SetMountSourceFolder(MountSourceFolder);

            TestContainer = await containerBuilder.StartContainer();
            await TestContainer.StopContainer();
            var connection = new MySqlConnection(TestContainer.Connection.ConnectionString);
            Action openConnection = () => connection.Open();

            openConnection.Should().Throw<MySqlException>().Where(e => e.ErrorCode == MySqlErrorCode.UnableToConnectToHost);

            TestContainer = await containerBuilder.StartContainer();

            var connectionString = TestContainer.Connection.ConnectionString;

            connectionString.Should().Contain($"Database={DatabaseName}");
            connectionString.Should().Contain($"Uid={UserName}");
            connectionString.Should().Contain($"Pwd={Password}");
        }
    }
}
