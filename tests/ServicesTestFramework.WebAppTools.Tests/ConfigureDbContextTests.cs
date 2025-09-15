using System.Net;
using ServicesTestFramework.ExampleApi;
using ServicesTestFramework.ExampleApi.Repositories;
using ServicesTestFramework.ExampleApi.Repositories.Entities;
using ServicesTestFramework.WebAppTools.Extensions;
using ServicesTestFramework.WebAppTools.Tests.Controllers;

namespace ServicesTestFramework.WebAppTools.Tests;

public class ConfigureDbContextTests : BaseTest
{
    [Test]
    public async Task Health_IsSuccessful_WithOKStatusCodeResponse()
    {
        var client = new WebApplicationBuilder<Startup>()
            .SwapDbContextWithInMemoryDatabase<TestDatabaseContext>()
            .CreateClient();

        var cosmosDbClient = client.ClientFor<ICosmosDbController>();

        var healthResponse = await cosmosDbClient.Health();

        healthResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task SwapDbContextWithInMemoryDatabase_AllowsToUseInMemoryDb()
    {
        var client = new WebApplicationBuilder<Startup>()
            .SwapDbContextWithInMemoryDatabase<TestDatabaseContext>()
            .CreateClient();

        var cosmosDbClient = client.ClientFor<ICosmosDbController>();

        var entityName = "Name";
        var entityIntValue = 42;

        var id = await cosmosDbClient.Add(entityName, entityIntValue);
        var newEntity = await cosmosDbClient.GetElement(id);

        var expected = new TestDatabaseEntity(id, entityName, entityIntValue);

        newEntity.Should().BeEquivalentTo(expected);
    }
}
