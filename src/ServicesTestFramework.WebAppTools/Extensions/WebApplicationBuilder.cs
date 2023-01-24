using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServicesTestFramework.WebAppTools.Authentication;
using Xunit.Abstractions;

namespace ServicesTestFramework.WebAppTools.Extensions;

public class WebApplicationBuilder<TEntryPoint> where TEntryPoint : class
{
    private WebApplicationFactory<TEntryPoint> Factory { get; }
    private List<Action<IServiceCollection>> ServicesConfigurations { get; } = new List<Action<IServiceCollection>>();
    private List<Action<IConfigurationBuilder>> ConfigureAppConfigurations { get; } = new List<Action<IConfigurationBuilder>>();
    private Dictionary<string, string> InMemoryCollectionConfig { get; } = new Dictionary<string, string>();

    private InMemoryDatabaseRoot InMemoryDatabaseRootObject { get; set; }

    public WebApplicationBuilder() => Factory = new WebApplicationFactory<TEntryPoint>();

    private WebApplicationBuilder(WebApplicationFactory<TEntryPoint> factory) => Factory = factory;

    /// <summary>
    /// Removes all services in <see cref="IServiceCollection"/> with the same service type
    /// as <typeparamref name="TService"/> and replaces them with <paramref name="implementationInstance"/>.
    /// <para>If specified services not found in <see cref="IServiceCollection"/>, exception thrown.</para>
    /// </summary>
    public WebApplicationBuilder<TEntryPoint> Swap<TService>(TService implementationInstance)
        where TService : class
    {
        ServicesConfigurations.Add(services => services.Swap<TService>(implementationInstance));

        return this;
    }

    /// <summary>
    /// Removes all services in <see cref="IServiceCollection"/> with the same service type
    /// as <typeparamref name="TService"/> and replaces them with <typeparamref name="TImplementation"/>.
    /// <para>If specified services not found in <see cref="IServiceCollection"/>, exception thrown.</para>
    /// </summary>
    public WebApplicationBuilder<TEntryPoint> Swap<TService, TImplementation>()
        where TService : class
        where TImplementation : class, TService
    {
        ServicesConfigurations.Add(services => services.Swap<TService, TImplementation>());

        return this;
    }

    /// <summary>
    /// Removes all services in <see cref="IServiceCollection"/> with the same service type
    /// as <typeparamref name="TService"/> and replaces them with <paramref name="implementationFactory"/>.
    /// <para>If specified services not found in <see cref="IServiceCollection"/>, exception thrown.</para>
    /// </summary>
    public WebApplicationBuilder<TEntryPoint> Swap<TService>(Func<IServiceProvider, TService> implementationFactory)
        where TService : class
    {
        ServicesConfigurations.Add(services => services.Swap<TService>(implementationFactory));

        return this;
    }

    /// <summary>
    /// Replaces existing <typeparamref name="TContext"/> in <see cref="IServiceCollection"/> with context
    /// configured by <paramref name="dbContextConfiguration"/>.
    /// </summary>
    public WebApplicationBuilder<TEntryPoint> SwapDbContext<TContext>(Action<DbContextOptionsBuilder> dbContextConfiguration)
        where TContext : DbContext
    {
        ServicesConfigurations.Add(services => services.SwapDbContext<TContext>(dbContextConfiguration));

        return this;
    }

    /// <summary>
    /// Configures EntityFramework context <typeparamref name="TContext"/> to connect to a named in-memory database.
    /// </summary>
    public WebApplicationBuilder<TEntryPoint> SwapDbContextWithInMemoryDatabase<TContext>(string databaseName = null)
        where TContext : DbContext
    {
        InMemoryDatabaseRootObject ??= new InMemoryDatabaseRoot();
        return SwapDbContext<TContext>(builder => builder.UseInMemoryDatabase(databaseName ?? "InMemoryDatabase", InMemoryDatabaseRootObject));
    }

    /// <summary>
    /// Apply changes to webApp services. Services can be added, deleted, swapped with mocks, etc.
    /// Changes applied after Startup#ConfigureServices.
    /// </summary>
    public WebApplicationBuilder<TEntryPoint> ConfigureServices(Action<IServiceCollection> servicesConfiguration)
    {
        ServicesConfigurations.Add(servicesConfiguration);

        return this;
    }

    /// <summary>
    /// Mocks authentication. Use in combination with <see cref="FakeToken"/>.
    /// </summary>
    /// <param name="authScheme">Authentication scheme to override.</param>
    public WebApplicationBuilder<TEntryPoint> AddMockAuthentication(string authScheme = JwtBearerDefaults.AuthenticationScheme)
    {
        ServicesConfigurations.Add(services => services.AddMockAuthentication(authScheme));

        return this;
    }

    /// <summary>
    /// Add/override webapp configuration value.
    /// </summary>
    public WebApplicationBuilder<TEntryPoint> AddConfiguration(string key, string value)
    {
        InMemoryCollectionConfig[key] = value;

        return this;
    }

    /// <summary>
    /// Add in-memory collection to webapp configuration.
    /// </summary>
    public WebApplicationBuilder<TEntryPoint> AddConfiguration(Dictionary<string, string> configCollection)
    {
        configCollection.ToList().ForEach(x => InMemoryCollectionConfig[x.Key] = x.Value);

        return this;
    }

    /// <summary>
    /// Add json configuration provider at <paramref name="path"/> to the webapp.
    /// </summary>
    /// <param name="path">Path relative to the base path stored in
    /// <see cref="IConfigurationBuilder.Properties"/>.</param>
    /// <param name="optional">Whether the file is optional.</param>
    /// <param name="reloadOnChange">Whether the configuration should be reloaded if the file changes.</param>
    public WebApplicationBuilder<TEntryPoint> AddConfiguration(string path, bool optional = false, bool reloadOnChange = false)
    {
        ConfigureAppConfigurations.Add(configBuilder => configBuilder.AddJsonFile(path, optional, reloadOnChange));

        return this;
    }

    /// <summary>
    /// Adds a delegate for configuring the <see cref="IConfigurationBuilder"/> for the webapp.
    /// </summary>
    public WebApplicationBuilder<TEntryPoint> AddConfiguration(Action<IConfigurationBuilder> configureAppConfiguration)
    {
        ConfigureAppConfigurations.Add(configureAppConfiguration);

        return this;
    }

    /// <summary>
    /// Adds an xUnit logger to the logging builder.
    /// </summary>
    /// <param name="outputHelper">The <see cref="ITestOutputHelper"/> to use.</param>
    /// <param name="logLevel">The minimum <see cref="LogLevel"/> to be logged.</param>
    public WebApplicationBuilder<TEntryPoint> AddXUnitLogger(ITestOutputHelper outputHelper, LogLevel logLevel = LogLevel.Debug) =>
        ConfigureLogging(logBuilder => logBuilder.AddXUnit(outputHelper,
            options => options.Filter = (_, level) => level >= logLevel));

    public WebApplicationBuilder<TEntryPoint> ConfigureLogging(Action<ILoggingBuilder> loggingConfiguration)
    {
        ServicesConfigurations.Add(services =>
        {
            services.AddLogging(loggingConfiguration);
        });

        return this;
    }

    /// <summary>
    /// Build WebApplicationFactory with provided configurations.
    /// </summary>
    public WebApplicationFactory<TEntryPoint> Build() =>
        Factory.WithWebHostBuilder(builder =>
        {
            builder.UseContentRoot(AppContext.BaseDirectory);

            foreach (var servicesConfiguration in ServicesConfigurations)
                builder.ConfigureTestServices(servicesConfiguration);

            builder.ConfigureAppConfiguration((_, configBuilder) =>
            {
                configBuilder
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                    .AddEnvironmentVariables();

                foreach (var configureAppConfiguration in ConfigureAppConfigurations)
                    configureAppConfiguration(configBuilder);

                configBuilder.AddInMemoryCollection(InMemoryCollectionConfig);
            });
        });

    /// <summary>
    /// Build WebApplicationFactory with provided configurations and create client.
    /// </summary>
    public HttpClient CreateClient() => Build().CreateClient();

    /// <summary>
    /// Build WebApplicationFactory with provided configurations and create client.
    /// </summary>
    public HttpClient CreateClient(WebApplicationFactoryClientOptions options) => Build().CreateClient(options);

    public static implicit operator WebApplicationFactory<TEntryPoint>(WebApplicationBuilder<TEntryPoint> builder) => builder.Build();

    public static implicit operator WebApplicationBuilder<TEntryPoint>(WebApplicationFactory<TEntryPoint> factory) => new WebApplicationBuilder<TEntryPoint>(factory);
}
