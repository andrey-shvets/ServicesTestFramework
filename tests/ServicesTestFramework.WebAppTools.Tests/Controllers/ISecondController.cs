using System.Net.Http;
using System.Threading.Tasks;
using RestEase;

namespace ServicesTestFramework.WebAppTools.Tests.Controllers
{
    [BasePath("second")]
    public interface ISecondController
    {
        [Get("getScopedServiceValue")]
        Task<string> GetScopedServiceValue();

        [Get("getSingletonServiceValue")]
        Task<string> GetSingletonServiceValue();

        [Get("getTransientServiceValue")]
        Task<string> GetTransientServiceValue();

        [Get("getConfigValue")]
        Task<string> GetConfigValue([Query] string configKey);

        [Get("health")]
        [AllowAnyStatusCode]
        Task<HttpResponseMessage> Health();
    }
}
