using ServicesTestFramework.DatabaseContainers.Containers;

namespace ServicesTestFramework.DatabaseContainers.Tests.ContainerTests;

public class MultipleContainersTest
{
    private const string DatabaseName = "testdb";
    private const string UserName = "testUser";
    private const string Password = "123456789";

    private MySqlTestContainer TestContainer { get; set; }
    private MySqlTestContainer AdditionalTestContainer { get; set; }

    [After(Test)]
    public async Task TestTeardown()
    {
        await TestContainer.StopContainer();
        await AdditionalTestContainer.StopContainer();
    }

    [Test]
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
