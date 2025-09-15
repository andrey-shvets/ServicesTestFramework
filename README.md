# Introduction

This repository provides tools for running ASP.NET Core services locally in isolation.

# Getting Started

- `ServicesTestFramework.WebAppTools` contains extension classes that help start and configure services for local execution.
- `ServicesTestFramework.DatabaseContainers` contains classes for starting Docker containers with a database and applying migrations to it.

## WebAppTools

Example
```
var factory = new WebApplicationBuilder<TEntryPoint>()
    .Swap<IPricingApiClient>(MockPricingApi.Object)
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

Used with Entity Framework.

`SwapDbContext<TContext>(Action<DbContextOptionsBuilder> dbContextConfiguration)`

Replaces the `DbContext` with a provided configuration.

`SwapDbContextWithInMemoryDatabase<TContext>(string databaseName = null)`

Replaces the `DbContext` with an in-memory database. 
This approach is not recommended. For reliable tests, they should be executed against the same database type and version as the production environment.

### AddConfiguration

`AddConfiguration(string path, bool optional = false, bool reloadOnChange = false)`

`AddConfiguration(string key, string value)`

`AddConfiguration(Dictionary<string, string> configCollection)`

### ConfigureLogging

`ConfigureLogging(Action<ILoggingBuilder> loggingConfiguration)`

### AddMockAuthentication

`AddMockAuthentication(string authScheme = JwtBearerDefaults.AuthenticationScheme)`

### Authentication Mock

`AddMockAuthentication()` replaces the existing authentication scheme with a mock one.
To mock claims use `FakeToken` class.
Example
```
var token = FakeToken.WithClaim(claimType).AndClaim(claimType, "42").AndClaim(additionalClaimType, "value").AndJwtId();
```

# Tests

To execute tests you need to have docker installed and running. Not all of the tests require docker but some do.

# Known Issues

The project containing the tests must have a **direct dependency** on `Microsoft.AspNetCore.Mvc.Testing`, despite the fact that there will be a transitive dependency from `ServicesTestFramework.WebAppTools`.