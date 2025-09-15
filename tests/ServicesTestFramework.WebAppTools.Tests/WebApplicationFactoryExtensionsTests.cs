using System.Net;
using ServicesTestFramework.ExampleApi;
using ServicesTestFramework.WebAppTools.Extensions;
using ServicesTestFramework.WebAppTools.Tests.Controllers;
using Xunit;

namespace ServicesTestFramework.WebAppTools.Tests;

public class WebApplicationFactoryExtensionsTests : BaseTest, IClassFixture<WebApplicationBuilder<Startup>>
{
    private WebApplicationBuilder<Startup> WebAppBuilder { get; } = new WebApplicationBuilder<Startup>();

    [Test]
    public async Task WithWebHostConfiguration_CreatesWorkingServiceThatCanBeAccessedThroughHttpClient()
    {
        var client = WebAppBuilder
            .CreateClient();

        var firstClient = client.ClientFor<IFirstController>();
        var secondClient = client.ClientFor<ISecondController>();

        var firstResponse = await firstClient.Health();
        var secondResponse = await secondClient.Health();

        firstResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        secondResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task WithWebHostConfiguration_AllowsToAddAppConfigurationSources()
    {
        Environment.SetEnvironmentVariable("TestOptions:TestKey", "valueFromEnvironmentShouldBeOverriden");
        Environment.SetEnvironmentVariable("TestOptions:EnvironmentTestKey", "value");

        var client = WebAppBuilder
            .AddConfiguration("appsettings.test.json")
            .AddConfiguration("InMemoryConfig", "inMemoryValue")
            .CreateClient();

        var secondClient = client.ClientFor<ISecondController>();

        var configValue = await secondClient.GetConfigValue("TestOptions:TestKey");
        var environmentConfigValue = await secondClient.GetConfigValue("TestOptions:EnvironmentTestKey");
        var inMemoryConfigValue = await secondClient.GetConfigValue("InMemoryConfig");

        configValue.Should().Be("valueFromJson");
        environmentConfigValue.Should().Be("value");
        inMemoryConfigValue.Should().Be("inMemoryValue");
    }
}
