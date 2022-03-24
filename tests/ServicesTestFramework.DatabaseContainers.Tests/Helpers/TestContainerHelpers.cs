using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using ServicesTestFramework.DatabaseContainers.Tests.Controllers;
using ServicesTestFramework.ExampleApi;
using ServicesTestFramework.WebAppTools.Extensions;

namespace ServicesTestFramework.DatabaseContainers.Tests.Helpers
{
    public static class TestContainerHelpers
    {
        public static string MountSourceFolder => $"mysqlData-{Guid.NewGuid()}";

        public static IDatabaseController StartWebAppWithClient(string connectionString)
        {
            var imMemoryConfig = new Dictionary<string, string>
            {
                { "Database:ConnectionString", connectionString }
            };

            var factory = new WebApplicationFactory<Startup>();

            var client = factory.WithWebHostConfiguration(
                    configureAppConfiguration: configBuilder => configBuilder.AddInMemoryCollection(imMemoryConfig))
                .CreateClient();

            var databaseClient = client.ClientFor<IDatabaseController>();

            return databaseClient;
        }
    }
}
