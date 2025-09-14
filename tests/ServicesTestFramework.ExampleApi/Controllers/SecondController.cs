using Microsoft.AspNetCore.Mvc;
using ServicesTestFramework.ExampleApi.Repositories.Interfaces;
using ServicesTestFramework.ExampleApi.Services.Interfaces;

namespace ServicesTestFramework.ExampleApi.Controllers;

[Route("[controller]")]
public class SecondController : ControllerBase
{
    private IConfiguration Configuration { get; }
    private ITestScopedService TestScopedServiceInstance { get; }
    private ITestSingletonService TestSingletonServiceInstance { get; }
    private ITestTransientService TestTransientServiceInstance { get; }
#pragma warning disable IDE0052 // Remove unread private members
    private ITestDao TestDao { get; }
#pragma warning restore IDE0052 // Remove unread private members

    public SecondController(
        IConfiguration configuration,
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

    [HttpGet("scopedServiceName")]
    public ActionResult<string> GetScopedServiceName()
    {
        var name = TestScopedServiceInstance.GetServiceName();

        return Ok(name);
    }

    [HttpGet("singletonServiceName")]
    public ActionResult<string> GetSingletonServiceName()
    {
        var name = TestSingletonServiceInstance.GetServiceName();

        return Ok(name);
    }

    [HttpGet("transientServiceName")]
    public ActionResult<string> GetTransientServiceName()
    {
        var name = TestTransientServiceInstance.GetServiceName();

        return Ok(name);
    }

    [HttpGet("getConfigValue")]
    public ActionResult<string> GetConfigValue([FromQuery] string configKey) => Configuration.GetSection(configKey).Get<string>();

    [HttpGet("health")]
    public ActionResult Health() => Ok();
}
