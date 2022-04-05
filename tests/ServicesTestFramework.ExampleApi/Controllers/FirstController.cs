using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ServicesTestFramework.ExampleApi.Services.Interfaces;

namespace ServicesTestFramework.ExampleApi.Controllers
{
    [Route("[controller]")]
    public class FirstController : ControllerBase
    {
        private IConfiguration Configuration { get; }
        private ITestScopedService TestScopedServiceInstance { get; }
        private ITestSingletonService TestSingletonServiceInstance { get; }
        private ITestTransientService TestTransientServiceInstance { get; }
        private IMultipleImplementationsService MultipleImplementationsServiceInstance { get; }

        public FirstController(IConfiguration configuration,
            ITestScopedService testScopedService,
            ITestSingletonService testSingletonService,
            ITestTransientService testTransientService,
            IMultipleImplementationsService multipleImplementationsService)
        {
            Configuration = configuration;
            TestScopedServiceInstance = testScopedService;
            TestSingletonServiceInstance = testSingletonService;
            TestTransientServiceInstance = testTransientService;
            MultipleImplementationsServiceInstance = multipleImplementationsService;
        }

        [HttpGet("getUserId")]
        [Authorize]
        public ActionResult<string> GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Ok(userId);
        }

        [HttpGet("getUserIdWithPolicy")]
        [Authorize(Policy = "TestPolicy")]
        public ActionResult<string> GetUserIdWithPolicy()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Ok(userId);
        }

        [HttpGet("getScopedServiceName")]
        public ActionResult<string> GetScopedServiceName()
        {
            var name = TestScopedServiceInstance.GetServiceName();

            return Ok(name);
        }

        [HttpGet("getSingletonServiceName")]
        public ActionResult<string> GetSingletonServiceName()
        {
            var name = TestSingletonServiceInstance.GetServiceName();

            return Ok(name);
        }

        [HttpGet("getTransientServiceName")]
        public ActionResult<string> GetTransientServiceName()
        {
            var name = TestTransientServiceInstance.GetServiceName();

            return Ok(name);
        }

        [HttpGet("getMultipleImplementationsServiceName")]
        public ActionResult<string> GetMultipleImplementationsServiceName()
        {
            var name = MultipleImplementationsServiceInstance.GetServiceName();

            return Ok(name);
        }

        [HttpGet("health")]
        [AllowAnonymous]
        public ActionResult Health() => Ok();
    }
}
