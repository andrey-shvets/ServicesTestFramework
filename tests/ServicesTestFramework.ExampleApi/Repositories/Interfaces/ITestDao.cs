using System.Collections.Generic;
using System.Threading.Tasks;
using ServicesTestFramework.ExampleApi.Repositories.Entities;

namespace ServicesTestFramework.ExampleApi.Repositories.Interfaces;

public interface ITestDao
{
    public Task<IEnumerable<FirstEntity>> GetFirst();

    public Task<IEnumerable<SecondEntity>> GetSecond();

    public Task<IEnumerable<ThirdEntity>> GetThird();

    public Task<IEnumerable<HotfixEntity>> GetHotfixTable();
}