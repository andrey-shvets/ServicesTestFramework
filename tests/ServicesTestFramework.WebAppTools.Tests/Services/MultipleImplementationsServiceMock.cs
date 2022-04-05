using ServicesTestFramework.ExampleApi.Services.Interfaces;

namespace ServicesTestFramework.WebAppTools.Tests.Services
{
    public class MultipleImplementationsServiceMock : IMultipleImplementationsService
    {
        public string GetServiceName() => "mockServiceName";
    }
}
