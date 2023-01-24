using FluentAssertions;
using ServicesTestFramework.DatabaseContainers.Docker;
using Xunit;

namespace ServicesTestFramework.DatabaseContainers.Tests.ContainerTests;

public class DockerHelperTests
{
    private const string DatabaseName = "testdb";
    private const string UserName = "testUser";
    private const string Password = "123456789";

    [Fact]
    public async Task GetMysqlPortForContainer_ReturnsPortOfMysqlDatabase_RunningInTheSpecifiedContainer()
    {
        var containerName = $"testContainer_{DateTime.Now.Ticks}";
        var testContainer = await new MySqlContainerBuilder()
                                .SetDatabaseConfiguration(DatabaseName, UserName, Password)
                                .SetContainerName(containerName)
                                .StartContainer();

        var testContainerConnectionString = testContainer.Connection.ConnectionString;

        var mysqlPort = DockerTools.GetMysqlPortForContainer(containerName);
        testContainerConnectionString.Should().Contain($"Port={mysqlPort}");

        var containerNameWithoutFirstCharacter = containerName.Remove(0, 1);
        Assert.Throws<InvalidOperationException>(() => DockerTools.GetMysqlPortForContainer(containerNameWithoutFirstCharacter));

        var containerNameWithoutLastCharacter = containerName.Remove(containerName.Length - 1, 1);
        Assert.Throws<InvalidOperationException>(() => DockerTools.GetMysqlPortForContainer(containerNameWithoutLastCharacter));
    }

    [Fact]
    public async Task GetMysqlPortForContainerByRegex_ReturnsPortOfMysqlDatabase_RunningInTheSpecifiedContainer()
    {
        var containerName = $"testContainerRegex_{DateTime.Now.Ticks}";
        var testContainer = await new MySqlContainerBuilder()
                                .SetDatabaseConfiguration(DatabaseName, UserName, Password)
                                .SetContainerName(containerName)
                                .StartContainer();

        var testContainerConnectionString = testContainer.Connection.ConnectionString;

        var containerNamePrefixPattern = @"^testContainer";
        var prefixPort = DockerTools.GetMysqlPortForContainerByRegex(containerNamePrefixPattern);
        testContainerConnectionString.Should().Contain($"Port={prefixPort}");

        var containerNameEndPattern = @"_\d+$";
        var endPort = DockerTools.GetMysqlPortForContainerByRegex(containerNameEndPattern);
        testContainerConnectionString.Should().Contain($"Port={endPort}");

        var nonExistentContainerPattern = @"x_testContainer";
        Assert.Throws<InvalidOperationException>(() => DockerTools.GetMysqlPortForContainerByRegex(nonExistentContainerPattern));
    }

    [Fact]
    public async Task RemoveContainerIfExists_StopsSpecifiedContainer()
    {
        var containerName = $"testStopContainer_{DateTime.Now.Ticks}";
        _ = await new MySqlContainerBuilder()
                .SetDatabaseConfiguration(DatabaseName, UserName, Password)
                .SetContainerName(containerName)
                .StartContainer();

        DockerTools.ContainerExists(containerName).Should().BeTrue();
        DockerTools.RemoveContainerIfExists(containerName);
        DockerTools.ContainerExists(containerName).Should().BeFalse();

        DockerTools.RemoveContainerIfExists(containerName);
    }

    [Fact]
    public async Task ContainerIsReusable_ReturnsTrueForRunningContainer()
    {
        var containerName = $"testReusableContainer_{DateTime.Now.Ticks}";
        _ = await new MySqlContainerBuilder()
                .SetDatabaseConfiguration(DatabaseName, UserName, Password)
                .SetContainerName(containerName)
                .StartContainer();

        DockerTools.ContainerIsReusable(containerName).Should().BeTrue();

        DockerHostTools.StopContainer(containerName);

        DockerTools.ContainerExists(containerName).Should().BeTrue();
        DockerTools.ContainerIsReusable(containerName).Should().BeFalse();

        DockerTools.RemoveContainerIfExists(containerName);
        DockerTools.ContainerIsReusable(containerName).Should().BeFalse();
    }
}
