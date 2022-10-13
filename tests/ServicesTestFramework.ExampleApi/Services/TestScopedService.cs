using ServicesTestFramework.ExampleApi.Services.Interfaces;

namespace ServicesTestFramework.ExampleApi.Services;

public class TestScopedService : ITestScopedService
{
    public string GetServiceName() => "actualScopedService";
}