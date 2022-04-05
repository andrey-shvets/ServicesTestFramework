using System.Net.Http;
using System.Threading.Tasks;
using RestEase;

namespace ServicesTestFramework.WebAppTools.Tests.Controllers
{
    [BasePath("second")]
    public interface ISecondController
    {
        [Get("getScopedServiceName")]
        Task<string> GetScopedServiceName();

        [Get("getSingletonServiceName")]
        Task<string> GetSingletonServiceName();

        [Get("getTransientServiceName")]
        Task<string> GetTransientServiceName();

        [Get("getConfigValue")]
        Task<string> GetConfigValue([Query] string configKey);

        [Get("health")]
        [AllowAnyStatusCode]
        Task<HttpResponseMessage> Health();
    }
}
