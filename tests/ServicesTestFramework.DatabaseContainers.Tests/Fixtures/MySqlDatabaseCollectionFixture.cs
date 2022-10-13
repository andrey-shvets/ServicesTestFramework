using Xunit;

namespace ServicesTestFramework.DatabaseContainers.Tests.Fixtures;

[CollectionDefinition(CollectionName)]
public class MySqlDatabaseCollectionFixture : ICollectionFixture<MySqlDatabaseFixture>
{
    public const string CollectionName = nameof(MySqlDatabaseFixture);
}
