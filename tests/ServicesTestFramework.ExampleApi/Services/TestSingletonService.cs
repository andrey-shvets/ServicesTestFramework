using ServicesTestFramework.ExampleApi.Services.Interfaces;

namespace ServicesTestFramework.ExampleApi.Services
{
    public class TestSingletonService : ITestSingletonService
    {
        public string GetServiceName() => "actualSingletonService";
    }
}
