using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServicesTestFramework.ExampleApi.Repositories.Interfaces;

namespace ServicesTestFramework.ExampleApi.Controllers
{
    [Route("[controller]")]
    public class DatabaseController : ControllerBase
    {
        private ITestDao TestDao { get; }

        public DatabaseController(ITestDao testDao)
        {
            TestDao = testDao;
        }

        [HttpGet("getFirstTableCount")]
        public async Task<ActionResult<int>> GetFirstTableCount()
        {
            var values = await TestDao.GetFirst();

            return Ok(values.Count());
        }

        [HttpGet("getSecondTableCount")]
        public async Task<ActionResult<int>> GetSecondTableCount()
        {
            var values = await TestDao.GetSecond();

            return Ok(values.Count());
        }

        [HttpGet("getThirdTableCount")]
        public async Task<ActionResult<int>> GetThirdTableCount()
        {
            var values = await TestDao.GetThird();

            return Ok(values.Count());
        }

        [HttpGet("getHotfixTableCount")]
        public async Task<ActionResult<int>> GetHotfixTableCount()
        {
            var values = await TestDao.GetHotfixTable();

            return Ok(values.Count());
        }

        [HttpGet("health")]
        public ActionResult Health() => Ok();
    }
}
