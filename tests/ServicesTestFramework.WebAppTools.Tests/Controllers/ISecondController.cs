using RestEase;

namespace ServicesTestFramework.WebAppTools.Tests.Controllers;

[BasePath("second")]
public interface ISecondController
{
    [Get("scopedServiceName")]
    Task<string> GetScopedServiceName();

    [Get("singletonServiceName")]
    Task<string> GetSingletonServiceName();

    [Get("transientServiceName")]
    Task<string> GetTransientServiceName();

    [Get("getConfigValue")]
    Task<string> GetConfigValue([Query] string configKey);

    [Get("health")]
    [AllowAnyStatusCode]
    Task<HttpResponseMessage> Health();
}
