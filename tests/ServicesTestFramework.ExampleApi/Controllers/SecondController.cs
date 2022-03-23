using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ServicesTestFramework.ExampleApi.Repositories.Interfaces;
using ServicesTestFramework.ExampleApi.Services.Interfaces;

namespace ServicesTestFramework.ExampleApi.Controllers
{
    [Route("[controller]")]
    public class SecondController : ControllerBase
    {
        private IConfiguration Configuration { get; }
        private ITestScopedService TestScopedServiceInstance { get; }
        private ITestSingletonService TestSingletonServiceInstance { get; }
        private ITestTransientService TestTransientServiceInstance { get; }
        private ITestDao TestDao { get; }

        public SecondController(IConfiguration configuration,
            ITestScopedService testScopedService,
            ITestSingletonService testSingletonService,
            ITestTransientService testTransientService,
            ITestDao testDao)
        {
            Configuration = configuration;
            TestScopedServiceInstance = testScopedService;
            TestSingletonServiceInstance = testSingletonService;
            TestTransientServiceInstance = testTransientService;
            TestDao = testDao;
        }

        [HttpGet("getScopedServiceValue")]
        public ActionResult<string> GetScopedServiceValue()
        {
            var name = TestScopedServiceInstance.GetServiceName();

            return Ok(name);
        }

        [HttpGet("getSingletonServiceValue")]
        public ActionResult<string> GetSingletonServiceValue()
        {
            var name = TestSingletonServiceInstance.GetServiceName();

            return Ok(name);
        }

        [HttpGet("getTransientServiceValue")]
        public ActionResult<string> GetTransientServiceValue()
        {
            var name = TestTransientServiceInstance.GetServiceName();

            return Ok(name);
        }

        [HttpGet("getConfigValue")]
        public ActionResult<string> GetConfigValue([FromQuery] string configKey) => Configuration.GetSection(configKey).Get<string>();

        [HttpGet("health")]
        public ActionResult Health() => Ok();
    }
}
