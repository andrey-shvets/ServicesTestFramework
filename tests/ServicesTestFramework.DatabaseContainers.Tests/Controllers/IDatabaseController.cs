using System.Net.Http;
using System.Threading.Tasks;
using RestEase;

namespace ServicesTestFramework.DatabaseContainers.Tests.Controllers;

[BasePath("database")]
public interface IDatabaseController
{
    [Get("getFirstTableCount")]
    Task<int> GetFirstTableCount();

    [Get("getSecondTableCount")]
    Task<int> GetSecondTableCount();

    [Get("getThirdTableCount")]
    Task<int> GetThirdTableCount();

    [Get("getHotfixTableCount")]
    Task<int> GetHotfixTableCount();

    [Get("health")]
    [AllowAnyStatusCode]
    Task<HttpResponseMessage> Health();
}