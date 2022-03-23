using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using ServicesTestFramework.ExampleApi;
using ServicesTestFramework.WebAppTools.Extensions;
using ServicesTestFramework.WebAppTools.Tests.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace ServicesTestFramework.WebAppTools.Tests
{
    public class WebApplicationFactoryExtensionsTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private WebApplicationFactory<Startup> ApplicationFactory { get; }
        private ITestOutputHelper OutputHelper { get; }

        public WebApplicationFactoryExtensionsTests(WebApplicationFactory<Startup> factory, ITestOutputHelper testOutputHelper)
        {
            ApplicationFactory = factory;
            OutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task WithWebHostConfiguration_CreatesWorkingServiceThatCanBeAccessedThroughHttpClient()
        {
            var client = ApplicationFactory.WithWebHostConfiguration(testOutputHelper: OutputHelper).CreateClient();
            var firstClient = client.ClientFor<IFirstController>();
            var secondClient = client.ClientFor<ISecondController>();

            var firstResponse = await firstClient.Health();
            var secondResponse = await secondClient.Health();

            firstResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            secondResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task WithWebHostConfiguration_AllowsToAddAppConfigurationSources()
        {
            var client = ApplicationFactory.WithWebHostConfiguration(
                configureAppConfiguration: configBuilder =>
                {
                    configBuilder.
                        AddJsonFile("appsettings.Test.json");
                },
                testOutputHelper: OutputHelper)
                .CreateClient();

            var secondClient = client.ClientFor<ISecondController>();

            var configValue = await secondClient.GetConfigValue("TestOptions:TestKey");

            configValue.Should().Be("value");
        }
    }
}
