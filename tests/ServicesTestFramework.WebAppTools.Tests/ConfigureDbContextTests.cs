using System.Net;
using FluentAssertions;
using ServicesTestFramework.ExampleApi;
using ServicesTestFramework.ExampleApi.Repositories;
using ServicesTestFramework.ExampleApi.Repositories.Entities;
using ServicesTestFramework.WebAppTools.Extensions;
using ServicesTestFramework.WebAppTools.Tests.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace ServicesTestFramework.WebAppTools.Tests;

public class ConfigureDbContextTests : BaseTest
{
    public ConfigureDbContextTests(ITestOutputHelper outputHelper)
        : base(outputHelper)
    {
    }

    [Fact]
    public async Task Health_IsSuccessful_WithOKStatusCodeResponse()
    {
        var client = new WebApplicationBuilder<Startup>()
            .AddXUnitLogger(OutputHelper)
            .SwapDbContextWithInMemoryDatabase<TestDatabaseContext>()
            .CreateClient();

        var cosmosDbClient = client.ClientFor<ICosmosDbController>();

        var healthResponse = await cosmosDbClient.Health();

        healthResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SwapDbContextWithInMemoryDatabase_AllowsToUseInMemoryDb()
    {
        var client = new WebApplicationBuilder<Startup>()
            .AddXUnitLogger(OutputHelper)
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