using ServicesTestFramework.DatabaseContainers.Tests.Controllers;
using ServicesTestFramework.DatabaseContainers.Tests.Fixtures;
using ServicesTestFramework.ExampleApi;
using ServicesTestFramework.WebAppTools.Extensions;

namespace ServicesTestFramework.DatabaseContainers.Tests;

public class DatabaseExampleTest
{
    private static IDatabaseController DatabaseClient { get; set; }

    [Before(Class)]
    public static void Setup()
    {
        var imMemoryConfig = new Dictionary<string, string>
        {
            { "Database:ConnectionString", MySqlDatabaseFixture.Connection.ConnectionString }
        };

        var client = new WebApplicationBuilder<Startup>()
            .AddConfiguration(imMemoryConfig)
            .CreateClient();

        DatabaseClient = client.ClientFor<IDatabaseController>();
    }

    [Test]
    public async Task MySqlDatabaseFixture_StartsDatabaseInContainer_WhichCanBeUsedAsSourceDatabaseForWebService()
    {
        var firstCount = await DatabaseClient.GetFirstTableCount();
        var secondCount = await DatabaseClient.GetSecondTableCount();
        var thirdCount = await DatabaseClient.GetThirdTableCount();
        var hotfixCount = await DatabaseClient.GetHotfixTableCount();

        firstCount.Should().Be(5);
        secondCount.Should().Be(4);
        thirdCount.Should().Be(6);
        hotfixCount.Should().Be(0);
    }
}
