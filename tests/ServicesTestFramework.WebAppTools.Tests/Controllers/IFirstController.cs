using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using RestEase;

namespace ServicesTestFramework.WebAppTools.Tests.Controllers;

[BasePath("first")]
public interface IFirstController
{
    [Get("getUserId")]
    Task<string> GetUserId([Header(nameof(HeaderNames.Authorization))] string authToken);

    [Get("getUserIdWithPolicy")]
    Task<string> GetUserIdWithPolicy([Header(nameof(HeaderNames.Authorization))] string authToken);

    [Get("getClaimByType")]
    Task<string> GetClaimByType([Query] string claimType, [Header(nameof(HeaderNames.Authorization))] string authToken);

    [Get("getClaimsByType")]
    Task<IEnumerable<string>> GetClaimsByType([Query] string claimType, [Header(nameof(HeaderNames.Authorization))] string authToken);

    [Get("getScopedServiceName")]
    Task<string> GetScopedServiceName();

    [Get("getSingletonServiceName")]
    Task<string> GetSingletonServiceName();

    [Get("getTransientServiceName")]
    Task<string> GetTransientServiceName();

    [Get("getMultipleImplementationsServiceName")]
    public Task<string> GetMultipleImplementationsServiceName();

    [Get("health")]
    [AllowAnyStatusCode]
    Task<HttpResponseMessage> Health();
}