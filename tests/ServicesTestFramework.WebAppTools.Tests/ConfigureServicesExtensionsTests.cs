using Microsoft.AspNetCore.Mvc.Testing;
using NSubstitute;
using ServicesTestFramework.ExampleApi;
using ServicesTestFramework.ExampleApi.Services;
using ServicesTestFramework.ExampleApi.Services.Interfaces;
using ServicesTestFramework.WebAppTools.Extensions;
using ServicesTestFramework.WebAppTools.Tests.Controllers;
using ServicesTestFramework.WebAppTools.Tests.Services;

namespace ServicesTestFramework.WebAppTools.Tests;

public class ConfigureServicesExtensionsTests : BaseTest
{
    private WebApplicationFactory<Startup> Factory { get; } = new WebApplicationFactory<Startup>();

    [Test]
    public void Swap_MissingServiceTypeWithInstance_ThrowsInvalidOperationExceptionOnClientCreation()
    {
        var builder = Factory.WithBuilder()
            .Swap<IAbsentService>(Substitute.For<IAbsentService>());

        Assert.Throws<InvalidOperationException>(() => builder.CreateClient());
    }

    [Test]
    public void Swap_MissingServiceTypeWithImplementation_ThrowsInvalidOperationExceptionOnClientCreation()
    {
        var builder = Factory.WithBuilder()
            .Swap<IAbsentService, AbsentService>();

        Assert.Throws<InvalidOperationException>(() => builder.CreateClient());
    }

    [Test]
    public void Swap_MissingServiceTypeWithImplementationFactory_ThrowsInvalidOperationExceptionOnClientCreation()
    {
        var builder = Factory.WithBuilder()
            .Swap<IAbsentService>(_ => Substitute.For<IAbsentService>());

        Assert.Throws<InvalidOperationException>(() => builder.CreateClient());
    }

    [Test]
    public async Task Swap_ChangesConfiguredServiceToProvidedInstance()
    {
        var scopedMock = Substitute.For<ITestScopedService>();
        var singletonMock = Substitute.For<ITestSingletonService>();
        var transientMock = Substitute.For<ITestTransientService>();

        var client = Factory.WithBuilder()
            .Swap<ITestScopedService>(scopedMock)
            .Swap<ITestSingletonService>(singletonMock)
            .Swap<ITestTransientService>(transientMock)
            .CreateClient();

        scopedMock.GetServiceName().Returns("mockScopedService");
        singletonMock.GetServiceName().Returns("mockSingletonService");
        transientMock.GetServiceName().Returns("mockTransientService");

        var firstClient = client.ClientFor<IFirstController>();

        var scopedValue = await firstClient.GetScopedServiceName();
        var singletonValue = await firstClient.GetSingletonServiceName();
        var transientValue = await firstClient.GetTransientServiceName();

        scopedValue.Should().Be("mockScopedService");
        singletonValue.Should().Be("mockSingletonService");
        transientValue.Should().Be("mockTransientService");
    }

    [Test]
    public async Task Swap_ChangesConfiguredServiceToProvidedImplementation()
    {
        var client = Factory.WithBuilder()
            .Swap<ITestScopedService, ScopedServiceMock>()
            .Swap<ITestSingletonService, SingletonServiceMock>()
            .Swap<ITestTransientService, TransientServiceMock>()
            .CreateClient();

        var firstClient = client.ClientFor<IFirstController>();

        var scopedValue = await firstClient.GetScopedServiceName();
        var singletonValue = await firstClient.GetSingletonServiceName();
        var transientValue = await firstClient.GetTransientServiceName();

        scopedValue.Should().Be("ScopedServiceMock");
        singletonValue.Should().Be("SingletonServiceMock");
        transientValue.Should().Be("TransientServiceMock");
    }

    [Test]
    public async Task Swap_ChangesConfiguredServiceToProvidedImplementationFactory()
    {
        var scopedMock = Substitute.For<ITestScopedService>();
        var singletonMock = Substitute.For<ITestSingletonService>();
        var transientMock = Substitute.For<ITestTransientService>();

        var client = Factory.WithBuilder()
            .Swap<ITestScopedService>(_ => scopedMock)
            .Swap<ITestSingletonService>(_ => singletonMock)
            .Swap<ITestTransientService>(_ => transientMock)
            .CreateClient();

        scopedMock.GetServiceName().Returns("mockScopedService");
        singletonMock.GetServiceName().Returns("mockSingletonService");
        transientMock.GetServiceName().Returns("mockTransientService");

        var firstClient = client.ClientFor<IFirstController>();

        var scopedValue = await firstClient.GetScopedServiceName();
        var singletonValue = await firstClient.GetSingletonServiceName();
        var transientValue = await firstClient.GetTransientServiceName();

        scopedValue.Should().Be("mockScopedService");
        singletonValue.Should().Be("mockSingletonService");
        transientValue.Should().Be("mockTransientService");
    }

    [Test]
    public void GetScopedService_RetrievesServiceInstanceFromWebAppFactory()
    {
        var scopedMock = Substitute.For<ITestScopedService>();
        var singletonMock = Substitute.For<ITestSingletonService>();
        var transientMock = Substitute.For<ITestTransientService>();

        scopedMock.GetServiceName().Returns("mockScopedService");
        singletonMock.GetServiceName().Returns("mockSingletonService");
        transientMock.GetServiceName().Returns("mockTransientService");

        var factory = Factory.WithBuilder()
            .Swap<ITestScopedService>(_ => scopedMock)
            .Swap<ITestSingletonService>(_ => singletonMock)
            .Swap<ITestTransientService>(_ => transientMock)
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

    [Test]
    public async Task Swap_ServiceWithMultipleImplementations_ChangesServiceToProvidedInstance()
    {
        var mock = Substitute.For<IMultipleImplementationsService>();

        mock.GetServiceName().Returns("mockServiceName");

        var client = Factory.WithBuilder()
            .Swap<IMultipleImplementationsService>(mock)
            .CreateClient();

        var firstClient = client.ClientFor<IFirstController>();

        var serviceName = await firstClient.GetMultipleImplementationsServiceName();

        serviceName.Should().Be("mockServiceName");
    }

    [Test]
    public async Task Swap_ServiceWithMultipleImplementations_ChangesServiceToProvidedImplementation()
    {
        var client = Factory.WithBuilder()
            .Swap<IMultipleImplementationsService, MultipleImplementationsServiceMock>()
            .CreateClient();

        var firstClient = client.ClientFor<IFirstController>();

        var serviceName = await firstClient.GetMultipleImplementationsServiceName();

        serviceName.Should().Be(MultipleImplementationsServiceMock.MockValue);
    }

    [Test]
    public async Task Swap_ServiceWithMultipleImplementations_ChangesServiceToProvidedImplementationFactory()
    {
        var mock = Substitute.For<IMultipleImplementationsService>();

        var client = Factory.WithBuilder()
            .Swap<IMultipleImplementationsService>(_ => mock)
            .CreateClient();

        mock.GetServiceName().Returns("mockServiceName");

        var firstClient = client.ClientFor<IFirstController>();

        var serviceName = await firstClient.GetMultipleImplementationsServiceName();

        serviceName.Should().Be("mockServiceName");
    }
}
