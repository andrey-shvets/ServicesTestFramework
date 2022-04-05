using ServicesTestFramework.ExampleApi.Services.Interfaces;

namespace ServicesTestFramework.ExampleApi.Services
{
    public class MultipleImplementationsExtraService : IMultipleImplementationsService
    {
        public string GetServiceName() => "MultipleImplementationsExtraService";
    }
}
