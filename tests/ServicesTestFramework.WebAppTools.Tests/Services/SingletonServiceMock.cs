using ServicesTestFramework.ExampleApi.Services.Interfaces;

namespace ServicesTestFramework.WebAppTools.Tests.Services;

public class SingletonServiceMock : ITestSingletonService
{
    public string GetServiceName() => "mockSingletonService";
}