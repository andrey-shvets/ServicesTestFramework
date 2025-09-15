using ServicesTestFramework.ExampleApi.Services.Interfaces;

namespace ServicesTestFramework.WebAppTools.Tests.Services;

public class MultipleImplementationsServiceMock : IMultipleImplementationsService
{
    public const string MockValue = "mockValue";

    public string GetServiceName() => MockValue;
}
