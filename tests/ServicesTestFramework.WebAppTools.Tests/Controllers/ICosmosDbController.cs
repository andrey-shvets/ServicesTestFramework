using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using RestEase;
using ServicesTestFramework.ExampleApi.Repositories.Entities;

namespace ServicesTestFramework.WebAppTools.Tests.Controllers
{
    [BasePath("cosmosDb")]
    public interface ICosmosDbController
    {
        [Post("add")]
        public Task<Guid> Add([Query] string name, int intData);

        [Get("get")]
        public Task<TestDatabaseEntity> Get([Query] Guid id);

        [Get("getAll")]
        public Task<IList<TestDatabaseEntity>> GetAll();

        [Get("health")]
        [AllowAnyStatusCode]
        public Task<HttpResponseMessage> Health();
    }
}
