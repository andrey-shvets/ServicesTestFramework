using System.Threading.Tasks;
using FluentAssertions;
using ServicesTestFramework.DatabaseContainers.Containers;
using Xunit;

namespace ServicesTestFramework.DatabaseContainers.Tests.ContainerTests
{
    public class MultipleContainersTest : IAsyncLifetime
    {
        private const string DatabaseName = "testdb";
        private const string UserName = "testUser";
        private const string Password = "123456789";

        private MySqlContainer TestContainer { get; set; }
        private MySqlContainer AdditionalTestContainer { get; set; }

        public Task InitializeAsync() => Task.CompletedTask;

        public async Task DisposeAsync()
        {
            await TestContainer.StopContainer();
            await AdditionalTestContainer.StopContainer();
        }

        [Fact]
        public async Task StartContainer_CanStartMultipleContainers_IfMountSourceFoldersAreDifferent()
        {
            TestContainer = await new MySqlContainerBuilder()
                                .SetDatabaseConfiguration(DatabaseName, UserName, Password)
                                .SetMountSourceFolder("mysqlData1")
                                .StartContainer();

            var connectionString = TestContainer.Connection.ConnectionString;

            connectionString.Should().Contain($"Database={DatabaseName}");
            connectionString.Should().Contain($"Uid={UserName}");
            connectionString.Should().Contain($"Pwd={Password}");

            AdditionalTestContainer = await new MySqlContainerBuilder()
                                          .SetDatabaseConfiguration(DatabaseName, UserName, Password)
                                          .SetMountSourceFolder("mysqlData2")
                                          .StartContainer();

            connectionString = TestContainer.Connection.ConnectionString;

            connectionString.Should().Contain($"Database={DatabaseName}");
            connectionString.Should().Contain($"Uid={UserName}");
            connectionString.Should().Contain($"Pwd={Password}");
        }
    }
}
