using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Ardalis.Specification;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ServicesTestFramework.ExampleApi.Repositories.Entities;

namespace ServicesTestFramework.ExampleApi.Controllers
{
    [Route("[controller]")]
    public class CosmosDbController : ControllerBase
    {
        private IRepositoryBase<TestDatabaseEntity> Repository { get; }

        public CosmosDbController(IRepositoryBase<TestDatabaseEntity> repository)
        {
            Repository = repository;
        }

        private void ConfigureLocalCosmosOptionsBuilder(CosmosDbContextOptionsBuilder builder)
        {
            builder.HttpClientFactory(() =>
            {
                HttpMessageHandler httpMessageHandler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                };

                return new HttpClient(httpMessageHandler);
            });

            builder.ConnectionMode(ConnectionMode.Gateway);
        }

        [HttpPost("add")]
        public async Task<ActionResult<Guid>> Add([FromQuery] string name, int intData)
        {
            var input = new TestDatabaseEntity(Guid.NewGuid(), name, intData);
            var newEntity = await Repository.AddAsync(input);

            return Ok(newEntity.Id);
        }

        [HttpGet("get")]
        public async Task<ActionResult<TestDatabaseEntity>> Get([FromQuery] Guid id)
        {
            var entity = await Repository.GetByIdAsync(id);

            return Ok(entity);
        }

        [HttpGet("getAll")]
        public async Task<ActionResult<IList<TestDatabaseEntity>>> GetAll()
        {
            var result = await Repository.ListAsync();

            return Ok(result);
        }

        [HttpGet("health")]
        public ActionResult Health() => Ok();
    }
}
