using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using ServicesTestFramework.DatabaseContainers.Tests.Controllers;
using ServicesTestFramework.DatabaseContainers.Tests.Fixtures;
using ServicesTestFramework.ExampleApi;
using ServicesTestFramework.WebAppTools.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace ServicesTestFramework.DatabaseContainers.Tests
{
    [Collection(MySqlDatabaseCollectionFixture.CollectionName)]
    public class DatabaseExampleTest : IClassFixture<WebApplicationBuilder<Startup>>
    {
        private IDatabaseController DatabaseClient { get; }

        public DatabaseExampleTest(
            MySqlDatabaseFixture mySqlDatabaseFixture,
            WebApplicationBuilder<Startup> builder,
            ITestOutputHelper testOutputHelper)
        {
            var imMemoryConfig = new Dictionary<string, string>
            {
                { "Database:ConnectionString", mySqlDatabaseFixture.Connection.ConnectionString }
            };

            var client = builder
                .AddConfiguration(imMemoryConfig)
                .AddXUnitLogger(testOutputHelper)
                .CreateClient();

            DatabaseClient = client.ClientFor<IDatabaseController>();
        }

        [Fact]
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
}
