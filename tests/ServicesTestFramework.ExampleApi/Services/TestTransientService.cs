using ServicesTestFramework.ExampleApi.Services.Interfaces;

namespace ServicesTestFramework.ExampleApi.Services
{
    public class TestTransientService : ITestTransientService
    {
        public string GetServiceName() => "actualTransientService";
    }
}
