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

        public FirstController(IConfiguration configuration,
            ITestScopedService testScopedService,
            ITestSingletonService testSingletonService,
            ITestTransientService testTransientService)
        {
            Configuration = configuration;
            TestScopedServiceInstance = testScopedService;
            TestSingletonServiceInstance = testSingletonService;
            TestTransientServiceInstance = testTransientService;
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

        [HttpGet("health")]
        [AllowAnonymous]
        public ActionResult Health() => Ok();
    }
}
