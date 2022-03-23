using System.Data.Common;
using DotNet.Testcontainers.Containers.Modules.Abstractions;

namespace ServicesTestFramework.DatabaseContainers
{
    public class DatabaseContainer
    {
        public TestcontainerDatabase Container { get; init; }
        public DbConnection Connection { get; init; }
    }
}
