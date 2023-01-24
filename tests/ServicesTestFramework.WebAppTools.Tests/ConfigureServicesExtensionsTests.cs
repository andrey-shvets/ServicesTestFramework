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

namespace ServicesTestFramework.WebAppTools.Tests;

public class ConfigureServicesExtensionsTests : BaseTest, IClassFixture<WebApplicationFactory<Startup>>
{
    private WebApplicationFactory<Startup> Factory { get; }

    public ConfigureServicesExtensionsTests(WebApplicationFactory<Startup> builder, ITestOutputHelper outputHelper)
        : base(outputHelper)
    {
        Factory = builder;
    }

    [Fact]
    public void Swap_MissingServiceTypeWithInstance_ThrowsInvalidOperationExceptionOnClientCreation()
    {
        var builder = Factory.WithBuilder()
            .Swap<IAbsentService>(Mock.Of<IAbsentService>())
            .AddXUnitLogger(OutputHelper);

        Assert.Throws<InvalidOperationException>(() => builder.CreateClient());
    }

    [Fact]
    public void Swap_MissingServiceTypeWithImplementation_ThrowsInvalidOperationExceptionOnClientCreation()
    {
        var builder = Factory.WithBuilder()
            .Swap<IAbsentService, AbsentService>()
            .AddXUnitLogger(OutputHelper);

        Assert.Throws<InvalidOperationException>(() => builder.CreateClient());
    }

    [Fact]
    public void Swap_MissingServiceTypeWithImplementationFactory_ThrowsInvalidOperationExceptionOnClientCreation()
    {
        var builder = Factory.WithBuilder()
            .Swap<IAbsentService>(_ => Mock.Of<IAbsentService>())
            .AddXUnitLogger(OutputHelper);

        Assert.Throws<InvalidOperationException>(() => builder.CreateClient());
    }

    [Fact]
    public async Task Swap_ChangesConfiguredServiceToProvidedInstance()
    {
        var client = Factory.WithBuilder()
            .Swap<ITestScopedService>(Mock.Of<ITestScopedService>(s => s.GetServiceName() == "mockScopedService"))
            .Swap<ITestSingletonService>(Mock.Of<ITestSingletonService>(s => s.GetServiceName() == "mockSingletonService"))
            .Swap<ITestTransientService>(Mock.Of<ITestTransientService>(s => s.GetServiceName() == "mockTransientService"))
            .AddXUnitLogger(OutputHelper)
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
        var client = Factory.WithBuilder()
            .Swap<ITestScopedService, ScopedServiceMock>()
            .Swap<ITestSingletonService, SingletonServiceMock>()
            .Swap<ITestTransientService, TransientServiceMock>()
            .AddXUnitLogger(OutputHelper)
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
        var client = Factory.WithBuilder()
            .Swap<ITestScopedService>(_ => Mock.Of<ITestScopedService>(s => s.GetServiceName() == "mockScopedService"))
            .Swap<ITestSingletonService>(_ => Mock.Of<ITestSingletonService>(s => s.GetServiceName() == "mockSingletonService"))
            .Swap<ITestTransientService>(_ => Mock.Of<ITestTransientService>(s => s.GetServiceName() == "mockTransientService"))
            .AddXUnitLogger(OutputHelper)
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
        var factory = Factory.WithBuilder()
            .Swap<ITestScopedService>(_ => Mock.Of<ITestScopedService>(s => s.GetServiceName() == "mockScopedService"))
            .Swap<ITestSingletonService>(_ => Mock.Of<ITestSingletonService>(s => s.GetServiceName() == "mockSingletonService"))
            .Swap<ITestTransientService>(_ => Mock.Of<ITestTransientService>(s => s.GetServiceName() == "mockTransientService"))
            .AddXUnitLogger(OutputHelper)
            .Build();

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
        var client = Factory.WithBuilder()
            .Swap<IMultipleImplementationsService>(Mock.Of<IMultipleImplementationsService>(s => s.GetServiceName() == "mockServiceName"))
            .AddXUnitLogger(OutputHelper)
            .CreateClient();

        var firstClient = client.ClientFor<IFirstController>();

        var serviceName = await firstClient.GetMultipleImplementationsServiceName();

        serviceName.Should().Be("mockServiceName");
    }

    [Fact]
    public async Task Swap_ServiceWithMultipleImplementations_ChangesServiceToProvidedImplementation()
    {
        var client = Factory.WithBuilder()
            .Swap<IMultipleImplementationsService, MultipleImplementationsServiceMock>()
            .AddXUnitLogger(OutputHelper)
            .CreateClient();

        var firstClient = client.ClientFor<IFirstController>();

        var serviceName = await firstClient.GetMultipleImplementationsServiceName();

        serviceName.Should().Be("mockServiceName");
    }

    [Fact]
    public async Task Swap_ServiceWithMultipleImplementations_ChangesServiceToProvidedImplementationFactory()
    {
        var client = Factory.WithBuilder()
            .Swap<IMultipleImplementationsService>(_ => Mock.Of<IMultipleImplementationsService>(s => s.GetServiceName() == "mockServiceName"))
            .AddXUnitLogger(OutputHelper)
            .CreateClient();

        var firstClient = client.ClientFor<IFirstController>();

        var serviceName = await firstClient.GetMultipleImplementationsServiceName();

        serviceName.Should().Be("mockServiceName");
    }
}
