using Ardalis.Specification;
using ServicesTestFramework.ExampleApi.Repositories.Entities;

namespace ServicesTestFramework.ExampleApi.Repositories;

public class TestDatabaseService : ITestDatabaseService
{
    private readonly IRepositoryBase<TestDatabaseEntity> _testDatabaseEntityRepository;

    public TestDatabaseService(
        IRepositoryBase<TestDatabaseEntity> testDatabaseEntityRepository)
    {
        this._testDatabaseEntityRepository = testDatabaseEntityRepository;
    }

    public async Task<TestDatabaseEntity> Add(string name, int intData)
    {
        var data = new TestDatabaseEntity(Guid.NewGuid(), name, intData);
        var newThing = await _testDatabaseEntityRepository.AddAsync(data);

        return newThing;
    }

    public async Task<TestDatabaseEntity> Get(Guid id)
    {
        var data = await _testDatabaseEntityRepository.GetByIdAsync(id);

        return data;
    }
}