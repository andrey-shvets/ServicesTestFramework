using System.Net;
using Docker.DotNet;
using MySqlConnector;
using ServicesTestFramework.DatabaseContainers.Containers;

namespace ServicesTestFramework.DatabaseContainers.Tests.ContainerTests;

public class MySqlContainerTests
{
    private const string DatabaseName = "testdb";
    private const string UserName = "testUser";
    private const string Password = "123456789";
    private MySqlTestContainer TestContainer { get; set; }

    [After(Test)]
    public async Task DisposeAsync()
    {
        if (TestContainer is not null)
            await TestContainer.StopContainer();
    }

    [Test]
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

    [Test]
    public async Task StopContainer_DisposesOfContainerAndAllowsToCreateNewContainerRunningFromTheSameMountFolder()
    {
        var containerBuilder = new MySqlContainerBuilder()
            .SetDatabaseConfiguration(DatabaseName, UserName, Password);

        TestContainer = await containerBuilder.StartContainer();

        var stopContainerTask = TestContainer.StopContainer();
        await stopContainerTask;

        var connection = new MySqlConnection(TestContainer.Connection.ConnectionString);
        Action openConnection = () => connection.Open();

        openConnection.Should().Throw<MySqlException>().Where(e => e.ErrorCode == MySqlErrorCode.UnableToConnectToHost);

        TestContainer = await containerBuilder.StartContainer();

        var connectionString = TestContainer.Connection.ConnectionString;

        connectionString.Should().Contain($"Database={DatabaseName}");
        connectionString.Should().Contain($"Uid={UserName}");
        connectionString.Should().Contain($"Pwd={Password}");
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    public void SetImageTagName_ThrowsArgumentException_ForInvalidImageTag(string image)
    {
        Assert.Throws<ArgumentException>(() => new MySqlContainerBuilder().SetImageTagName(image));
    }

    [Test]
    [Arguments("non/existing/image:9.8.7")]
    [Arguments("mysql:999.42.24")]
    public async Task SetImageTagName_ThrowsDockerImageNotFoundException_ForNonExistentImage(string image)
    {
        var ex = await Assert.ThrowsAsync<DockerApiException>(async () => await new MySqlContainerBuilder()
            .SetDatabaseConfiguration(DatabaseName, UserName, Password)
            .SetImageTagName(image)
            .StartContainer());

        await Assert.That(ex.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }
}
