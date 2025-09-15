using RestEase;
using ServicesTestFramework.ExampleApi.Repositories.Entities;

namespace ServicesTestFramework.WebAppTools.Tests.Controllers;

[BasePath("cosmosDb")]
public interface ICosmosDbController
{
    [Post("add")]
    Task<Guid> Add([Query] string name, int intData);

    [Get("get")]
    Task<TestDatabaseEntity> GetElement([Query] Guid id);

    [Get("getAll")]
    Task<IList<TestDatabaseEntity>> GetAll();

    [Get("health")]
    [AllowAnyStatusCode]
    Task<HttpResponseMessage> Health();
}
