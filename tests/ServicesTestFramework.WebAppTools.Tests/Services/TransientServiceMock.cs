using ServicesTestFramework.ExampleApi.Services.Interfaces;

namespace ServicesTestFramework.WebAppTools.Tests.Services;

public class TransientServiceMock : ITestTransientService
{
    public string GetServiceName() => "mockTransientService";
}