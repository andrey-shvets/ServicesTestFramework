using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using ServicesTestFramework.ExampleApi;
using ServicesTestFramework.ExampleApi.Services;
using ServicesTestFramework.ExampleApi.Services.Interfaces;
using ServicesTestFramework.WebAppTools.Extensions;
using ServicesTestFramework.WebAppTools.Tests.Controllers;
using ServicesTestFramework.WebAppTools.Tests.Services;
using Xunit;
using Xunit.Abstractions;

namespace ServicesTestFramework.WebAppTools.Tests
{
    public class ConfigureServicesExtensionsTests : BaseTest, IClassFixture<WebApplicationFactory<Startup>>
    {
        private WebApplicationFactory<Startup> ApplicationFactory { get; }

        public ConfigureServicesExtensionsTests(WebApplicationFactory<Startup> factory, ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
            ApplicationFactory = factory;
        }

        [Fact]
        public void Swap_MissingServiceTypeWithInstance_ThrowsInvalidOperationExceptionOnClientCreation()
        {
            var factory = ApplicationFactory.WithWebHostConfiguration(
                servicesConfiguration: services =>
                {
                    services.Swap<IAbsentService>(Mock.Of<IAbsentService>());
                },
                testOutputHelper: OutputHelper);

            Assert.Throws<InvalidOperationException>(() => factory.CreateClient());
        }

        [Fact]
        public void Swap_MissingServiceTypeWithImplementation_ThrowsInvalidOperationExceptionOnClientCreation()
        {
            var factory = ApplicationFactory.WithWebHostConfiguration(
                servicesConfiguration: services =>
                {
                    services.Swap<IAbsentService, AbsentService>();
                },
                testOutputHelper: OutputHelper);

            Assert.Throws<InvalidOperationException>(() => factory.CreateClient());
        }

        [Fact]
        public void Swap_MissingServiceTypeWithImplementationFactory_ThrowsInvalidOperationExceptionOnClientCreation()
        {
            var factory = ApplicationFactory.WithWebHostConfiguration(
                servicesConfiguration: services =>
                {
                    services.Swap<IAbsentService>(_ => Mock.Of<IAbsentService>());
                },
                testOutputHelper: OutputHelper);

            Assert.Throws<InvalidOperationException>(() => factory.CreateClient());
        }

        [Fact]
        public async Task Swap_ChangesConfiguredServiceToProvidedInstance()
        {
            var client = ApplicationFactory.WithWebHostConfiguration(
                    servicesConfiguration: services =>
                    {
                        services.Swap<ITestScopedService>(Mock.Of<ITestScopedService>(s => s.GetServiceName() == "mockScopedService"));
                        services.Swap<ITestSingletonService>(Mock.Of<ITestSingletonService>(s => s.GetServiceName() == "mockSingletonService"));
                        services.Swap<ITestTransientService>(Mock.Of<ITestTransientService>(s => s.GetServiceName() == "mockTransientService"));
                    },
                    testOutputHelper: OutputHelper)
                .CreateClient();

            var firstClient = client.ClientFor<IFirstController>();

            var scopedValue = await firstClient.GetScopedServiceName();
            var singletonValue = await firstClient.GetSingletonServiceName();
            var transientValue = await firstClient.GetTransientServiceName();

            scopedValue.Should().Be("mockScopedService");
            singletonValue.Should().Be("mockSingletonService");
            transientValue.Should().Be("mockTransientService");
        }

        [Fact]
        public async Task Swap_ChangesConfiguredServiceToProvidedImplementation()
        {
            var client = ApplicationFactory.WithWebHostConfiguration(
                    servicesConfiguration: services =>
                    {
                        services.Swap<ITestScopedService, ScopedServiceMock>();
                        services.Swap<ITestSingletonService, SingletonServiceMock>();
                        services.Swap<ITestTransientService, TransientServiceMock>();
                    },
                    testOutputHelper: OutputHelper)
                .CreateClient();

            var firstClient = client.ClientFor<IFirstController>();

            var scopedValue = await firstClient.GetScopedServiceName();
            var singletonValue = await firstClient.GetSingletonServiceName();
            var transientValue = await firstClient.GetTransientServiceName();

            scopedValue.Should().Be("mockScopedService");
            singletonValue.Should().Be("mockSingletonService");
            transientValue.Should().Be("mockTransientService");
        }

        [Fact]
        public async Task Swap_ChangesConfiguredServiceToProvidedImplementationFactory()
        {
            var client = ApplicationFactory.WithWebHostConfiguration(
                    servicesConfiguration: services =>
                    {
                        services.Swap<ITestScopedService>(_ => Mock.Of<ITestScopedService>(s => s.GetServiceName() == "mockScopedService"));
                        services.Swap<ITestSingletonService>(_ => Mock.Of<ITestSingletonService>(s => s.GetServiceName() == "mockSingletonService"));
                        services.Swap<ITestTransientService>(_ => Mock.Of<ITestTransientService>(s => s.GetServiceName() == "mockTransientService"));
                    },
                    testOutputHelper: OutputHelper)
                .CreateClient();

            var firstClient = client.ClientFor<IFirstController>();

            var scopedValue = await firstClient.GetScopedServiceName();
            var singletonValue = await firstClient.GetSingletonServiceName();
            var transientValue = await firstClient.GetTransientServiceName();

            scopedValue.Should().Be("mockScopedService");
            singletonValue.Should().Be("mockSingletonService");
            transientValue.Should().Be("mockTransientService");
        }

        [Fact]
        public void GetScopedService_RetrievesServiceInstanceFromWebAppFactory()
        {
            var factory = ApplicationFactory.WithWebHostConfiguration(
                servicesConfiguration: services =>
                {
                    services.Swap<ITestScopedService>(_ => Mock.Of<ITestScopedService>(s => s.GetServiceName() == "mockScopedService"));
                    services.Swap<ITestSingletonService>(_ => Mock.Of<ITestSingletonService>(s => s.GetServiceName() == "mockSingletonService"));
                    services.Swap<ITestTransientService>(_ => Mock.Of<ITestTransientService>(s => s.GetServiceName() == "mockTransientService"));
                },
                testOutputHelper: OutputHelper);

            var scopedService = factory.Services.GetScopedService<ITestScopedService>();
            var singletonService = factory.Services.GetScopedService<ITestSingletonService>();
            var transientService = factory.Services.GetScopedService<ITestTransientService>();

            var scopedServiceName = scopedService.GetServiceName();
            var singletonServiceName = singletonService.GetServiceName();
            var transientServiceName = transientService.GetServiceName();

            scopedServiceName.Should().Be("mockScopedService");
            singletonServiceName.Should().Be("mockSingletonService");
            transientServiceName.Should().Be("mockTransientService");
        }

        [Fact]
        public async Task Swap_ServiceWithMultipleImplementations_ChangesServiceToProvidedInstance()
        {
            var client = ApplicationFactory.WithWebHostConfiguration(
                    servicesConfiguration: services =>
                    {
                        services.Swap<IMultipleImplementationsService>(Mock.Of<IMultipleImplementationsService>(s => s.GetServiceName() == "mockServiceName"));
                    },
                    testOutputHelper: OutputHelper)
                .CreateClient();

            var firstClient = client.ClientFor<IFirstController>();

            var serviceName = await firstClient.GetMultipleImplementationsServiceName();

            serviceName.Should().Be("mockServiceName");
        }

        [Fact]
        public async Task Swap_ServiceWithMultipleImplementations_ChangesServiceToProvidedImplementation()
        {
            var client = ApplicationFactory.WithWebHostConfiguration(
                    servicesConfiguration: services =>
                    {
                        services.Swap<IMultipleImplementationsService, MultipleImplementationsServiceMock>();
                    },
                    testOutputHelper: OutputHelper)
                .CreateClient();

            var firstClient = client.ClientFor<IFirstController>();

            var serviceName = await firstClient.GetMultipleImplementationsServiceName();

            serviceName.Should().Be("mockServiceName");
        }

        [Fact]
        public async Task Swap_ServiceWithMultipleImplementations_ChangesServiceToProvidedImplementationFactory()
        {
            var client = ApplicationFactory.WithWebHostConfiguration(
                    servicesConfiguration: services =>
                    {
                        services.Swap<IMultipleImplementationsService>(_ => Mock.Of<IMultipleImplementationsService>(s => s.GetServiceName() == "mockServiceName"));
                    },
                    testOutputHelper: OutputHelper)
                .CreateClient();

            var firstClient = client.ClientFor<IFirstController>();

            var serviceName = await firstClient.GetMultipleImplementationsServiceName();

            serviceName.Should().Be("mockServiceName");
        }
    }
}
