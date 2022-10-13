namespace ServicesTestFramework.DatabaseContainers.Tests.Fixtures;

internal class TestEnvironmentOptions
{
    public const string Environment = "TestEnvironment";

    /// <summary>
    /// Specifies MySqImage to run in the docker container, e.g 'mysql:8.0.18', if value not set, latest image is used.
    /// </summary>
    public string MysqlImageTagName { get; set; }

    /// <summary>
    /// If true, before container with database initialized, DB is restored from snapshot file with some of the migrations already applied.
    /// </summary>
    public bool InitializeDatabaseFromSnapshot { get; set; }

    public string TestContainerName { get; set; } = "test_container";

    /// <summary>
    /// <para>
    /// When set to <see langword="true"/>, on the start of the test run, if the container container with the name specified in <see cref="TestContainerName"/> is running.
    /// </para>
    /// <para>
    /// When set to <see langword="false"/>, do not stop container after tests are finished. On the start of the test run
    /// connect to the DB in the running container with name specified in <see cref="TestContainerName"/>.
    /// </para>
    /// </summary>
    public bool RestartDatabaseContainer { get; set; }
}