using System.Net;
using Docker.DotNet;
using FluentAssertions;
using MySqlConnector;
using ServicesTestFramework.DatabaseContainers.Containers;
using Xunit;

namespace ServicesTestFramework.DatabaseContainers.Tests.ContainerTests;

public class MySqlContainerTests : IAsyncLifetime
{
    private const string DatabaseName = "testdb";
    private const string UserName = "testUser";
    private const string Password = "123456789";
    private MySqlTestContainer TestContainer { get; set; }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        if (TestContainer is not null)
            await TestContainer.StopContainer();
    }

    [Fact]
    public async Task StartContainer_CreatesContainerWithDatabaseInitialized()
    {
        TestContainer = await new MySqlContainerBuilder()
                            .SetDatabaseConfiguration(DatabaseName, UserName, Password)
                            .SetImageTagName("mysql:8.0.28")
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
            .SetDatabaseConfiguration(DatabaseName, UserName, Password);

        TestContainer = await containerBuilder.StartContainer();

        var stopContainerTask = TestContainer.StopContainer();
#pragma warning disable VSTHRD103 // Call async methods when in an async method
        stopContainerTask.Wait();
#pragma warning restore VSTHRD103 // Call async methods when in an async method

        var connection = new MySqlConnection(TestContainer.Connection.ConnectionString);
        Action openConnection = () => connection.Open();

        openConnection.Should().Throw<MySqlException>().Where(e => e.ErrorCode == MySqlErrorCode.UnableToConnectToHost);

        TestContainer = await containerBuilder.StartContainer();

        var connectionString = TestContainer.Connection.ConnectionString;

        connectionString.Should().Contain($"Database={DatabaseName}");
        connectionString.Should().Contain($"Uid={UserName}");
        connectionString.Should().Contain($"Pwd={Password}");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void SetImageTagName_ThrowsArgumentException_ForInvalidImageTag(string image)
    {
        _ = Assert.Throws<ArgumentException>(() => new MySqlContainerBuilder().SetImageTagName(image));
    }

    [Theory]
    [InlineData("non/existing/image:9.8.7")]
    [InlineData("mysql:999.42.24")]
    public async Task SetImageTagName_ThrowsDockerImageNotFoundException_ForNonExistentImage(string image)
    {
        var ex = await Assert.ThrowsAsync<DockerApiException>(async () => await new MySqlContainerBuilder()
                                                                                        .SetDatabaseConfiguration(DatabaseName, UserName, Password)
                                                                                        .SetImageTagName(image)
                                                                                        .StartContainer());

        ex.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
