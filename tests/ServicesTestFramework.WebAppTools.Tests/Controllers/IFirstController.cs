using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using RestEase;

namespace ServicesTestFramework.WebAppTools.Tests.Controllers
{
    [BasePath("first")]
    public interface IFirstController
    {
        [Get("getUserId")]
        Task<string> GetUserId([Header(nameof(HeaderNames.Authorization))] string authToken);

        [Get("getUserIdWithPolicy")]
        Task<string> GetUserIdWithPolicy([Header(nameof(HeaderNames.Authorization))] string authToken);

        [Get("getScopedServiceValue")]
        Task<string> GetScopedServiceValue();

        [Get("getSingletonServiceValue")]
        Task<string> GetSingletonServiceValue();

        [Get("getTransientServiceValue")]
        Task<string> GetTransientServiceValue();

        [Get("health")]
        [AllowAnyStatusCode]
        Task<HttpResponseMessage> Health();
    }
}
