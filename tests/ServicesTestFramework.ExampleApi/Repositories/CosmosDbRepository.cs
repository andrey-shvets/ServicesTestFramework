using Ardalis.Specification.EntityFrameworkCore;

namespace ServicesTestFramework.ExampleApi.Repositories;

public class CosmosDbRepository<T> : RepositoryBase<T> where T : class
{
    private TestDatabaseContext Context { get; }

    public CosmosDbRepository(TestDatabaseContext dbContext) : base(dbContext)
    {
        Context = dbContext;
    }

    public async Task<T> Add(T input)
    {
        var newEntity = await Context.AddAsync(input);

        return newEntity.Entity;
    }
}
