using Microsoft.Net.Http.Headers;
using RestEase;

namespace ServicesTestFramework.WebAppTools.Tests.Controllers;

[BasePath("first")]
public interface IFirstController
{
    [Get("userId")]
    Task<string> GetUserId([Header(nameof(HeaderNames.Authorization))] string authToken);

    [Get("userIdWithPolicy")]
    Task<string> GetUserIdWithPolicy([Header(nameof(HeaderNames.Authorization))] string authToken);

    [Get("claimByType")]
    Task<string> GetClaimByType([Query] string claimType, [Header(nameof(HeaderNames.Authorization))] string authToken);

    [Get("claimsByType")]
    Task<IEnumerable<string>> GetClaimsByType([Query] string claimType, [Header(nameof(HeaderNames.Authorization))] string authToken);

    [Get("scopedServiceName")]
    Task<string> GetScopedServiceName();

    [Get("singletonServiceName")]
    Task<string> GetSingletonServiceName();

    [Get("transientServiceName")]
    Task<string> GetTransientServiceName();

    [Get("multipleImplementationsServiceName")]
    Task<string> GetMultipleImplementationsServiceName();

    [Get("health")]
    [AllowAnyStatusCode]
    Task<HttpResponseMessage> Health();
}
