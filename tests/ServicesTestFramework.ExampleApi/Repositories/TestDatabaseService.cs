using Ardalis.Specification;
using ServicesTestFramework.ExampleApi.Repositories.Entities;

namespace ServicesTestFramework.ExampleApi.Repositories;

public class TestDatabaseService : ITestDatabaseService
{
    private IRepositoryBase<TestDatabaseEntity> TestDatabaseEntityRepository { get; set; }

    public TestDatabaseService(
        IRepositoryBase<TestDatabaseEntity> testDatabaseEntityRepository)
    {
        TestDatabaseEntityRepository = testDatabaseEntityRepository;
    }

    public async Task<TestDatabaseEntity> Add(string name, int intData)
    {
        var data = new TestDatabaseEntity(Guid.NewGuid(), name, intData);
        var newThing = await TestDatabaseEntityRepository.AddAsync(data);

        return newThing;
    }

    public async Task<TestDatabaseEntity> Get(Guid id)
    {
        var data = await TestDatabaseEntityRepository.GetByIdAsync(id);

        return data;
    }
}
