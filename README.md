# Introduction 
Tools for ruuning ASP.NET Core services locally in isolation. 

# Getting Started
- `ServicesTestFramework.WebAppTools` contains extension classes that help start and configure service to run locally.
- `ServicesTestFramework.DatabaseContainers` contains classes for starting docker container with DB and applying migrations to that DB.

# WebAppTools

Example
```
var factory = new WebApplicationBuilder<TEntryPoint>()
                .Swap<IPricingApiClient>(MockPricingApi.Object);
                .Swap<IVehicleCommonInfrastructure, TestVehicleCommonInfrastructure>()
                .AddConfiguration("appsettings.tests.json", optional: true, reloadOnChange: false)
                .AddConfiguration("DbConnectionString", connectionString)
                .AddMockAuthentication()
                .Build();
```

### Swap
`Swap<TService>(TService implementationInstance)`
`Swap<TService, TImplementation>()`
`Swap<TService>(Func<IServiceProvider, TService> implementationFactory)`

### SwapDbContext
Used with EntityFramework.

`SwapDbContext<TContext>(Action<DbContextOptionsBuilder> dbContextConfiguration)`
Replaces DbContext with provided configuration.

`SwapDbContextWithInMemoryDatabase<TContext>(string databaseName = null)`
Replaces DbContext with in-memory database.
This is not recomended approach. To have reliavle tests they should be executed against the same type/version of database as WebApp connected on production. 

### AddConfiguration

`AddConfiguration(string path, bool optional = false, bool reloadOnChange = false)`
`AddConfiguration(string key, string value)`
`AddConfiguration(Dictionary<string, string> configCollection)`

### ConfigureLogging
`ConfigureLogging(Action<ILoggingBuilder> loggingConfiguration)`

### AddMockAuthentication
`AddMockAuthentication(string authScheme = JwtBearerDefaults.AuthenticationScheme)`

## Authentication Mock



# Known issues
- The project containing the tests should have a direct dependency on `Microsoft.AspNetCore.Mvc.Testing` despite that there is transitive dependency from `ServicesTestFramework.WebAppTools`.
