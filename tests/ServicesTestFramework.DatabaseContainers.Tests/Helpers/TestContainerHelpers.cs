using ServicesTestFramework.DatabaseContainers.Tests.Controllers;
using ServicesTestFramework.ExampleApi;
using ServicesTestFramework.WebAppTools.Extensions;

namespace ServicesTestFramework.DatabaseContainers.Tests.Helpers
{
    public static class TestContainerHelpers
    {
        public static IDatabaseController StartWebAppWithClient(string connectionString)
        {
            var client = new WebApplicationBuilder<Startup>()
                .AddConfiguration("Database:ConnectionString", connectionString)
                .CreateClient();

            var databaseClient = client.ClientFor<IDatabaseController>();

            return databaseClient;
        }
    }
}
