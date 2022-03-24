using Xunit;

namespace ServicesTestFramework.DatabaseContainers.Tests.Fixtures
{
    [CollectionDefinition(CollectionName)]
    public class MySqlDatabaseCollection : ICollectionFixture<MySqlDatabaseFixture>
    {
        public const string CollectionName = nameof(MySqlDatabaseCollection);
    }
}
