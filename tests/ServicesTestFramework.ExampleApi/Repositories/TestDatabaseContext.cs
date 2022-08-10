using Microsoft.EntityFrameworkCore;
using ServicesTestFramework.ExampleApi.Repositories.Entities;

namespace ServicesTestFramework.ExampleApi.Repositories
{
    public class TestDatabaseContext : DbContext
    {
        public DbSet<TestDatabaseEntity> TestDatabaseData => Set<TestDatabaseEntity>();

        public TestDatabaseContext(DbContextOptions<TestDatabaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestDatabaseEntity>().ToContainer("Test");
            modelBuilder.Entity<TestDatabaseEntity>().HasPartitionKey(b => b.Id);
        }
    }
}
