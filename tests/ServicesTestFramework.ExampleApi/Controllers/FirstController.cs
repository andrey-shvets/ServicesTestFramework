using System.Collections.Generic;
using System.Linq;
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

        [HttpGet("getClaimByType")]
        public ActionResult<string> GetClaimByType([FromQuery] string claimType)
        {
            var claims = User.FindAll(claimType).ToList();

            if (!claims.Any())
                return NotFound($"Failed to find any claims of type {claimType}");

            if (claims.Count > 1)
                return BadRequest($"{claims.Count} claims of type {claimType} were found, while only one was expected.");

            var claimValue = claims.First().Value;

            return Ok(claimValue);
        }

        [HttpGet("getClaimsByType")]
        public ActionResult<IEnumerable<string>> GetClaimsByType([FromQuery] string claimType)
        {
            var claims = User.FindAll(claimType).ToList();

            if (!claims.Any())
                return NotFound($"Failed to find any claims of type {claimType}");

            var claimValues = claims.Select(c => c.Value);

            return Ok(claimValues);
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
