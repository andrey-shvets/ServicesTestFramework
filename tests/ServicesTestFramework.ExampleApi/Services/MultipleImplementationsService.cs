using ServicesTestFramework.ExampleApi.Services.Interfaces;

namespace ServicesTestFramework.ExampleApi.Services
{
    public class MultipleImplementationsService : IMultipleImplementationsService
    {
        public string GetServiceName() => "MultipleImplementationsService";
    }
}
