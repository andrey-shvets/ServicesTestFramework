using ServicesTestFramework.ExampleApi.Services.Interfaces;

namespace ServicesTestFramework.WebAppTools.Tests.Services;

public class ScopedServiceMock : ITestScopedService
{
    public string GetServiceName() => nameof(ScopedServiceMock);
}
