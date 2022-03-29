using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestEase;
using Serilog;
using Serilog.Events;
using Xunit.Abstractions;

namespace ServicesTestFramework.WebAppTools.Extensions
{
    public static class WebApplicationFactoryExtensions
    {
        /// <summary>
        /// Configure the webapp before running it locally.
        /// </summary>
        /// <typeparam name="TStartup">Startup class of service under test</typeparam>
        /// <param name="factory">Application factory for service under test.</param>
        /// <param name="servicesConfiguration">Delegate for a method with any additional services configurations. Services can be added, deleted or swapped with mocks.</param>
        /// <param name="configureAppConfiguration">Delegate for configuring additional app configuration sources. `appsettings.json` and environment variables are added by default.</param>
        /// <param name="testOutputHelper">Object which can be used to provide test output.</param>
        public static WebApplicationFactory<TStartup> WithWebHostConfiguration<TStartup>(this WebApplicationFactory<TStartup> factory,
            Action<IServiceCollection> servicesConfiguration = null,
            Action<IConfigurationBuilder> configureAppConfiguration = null,
            ITestOutputHelper testOutputHelper = null)
            where TStartup : class
        {
            return factory.WithWebHostBuilder(builder =>
            {
                builder.InitializeLogger(testOutputHelper)
                    .UseContentRoot(AppContext.BaseDirectory);

                if (servicesConfiguration is not null)
                    builder.ConfigureTestServices(servicesConfiguration);

                builder.ConfigureAppConfiguration((_, configBuilder) =>
                {
                    configBuilder
                        .AddJsonFile("appsettings.json", optional: true)
                        .AddEnvironmentVariables();

                    if (configureAppConfiguration is not null)
                        configureAppConfiguration(configBuilder);
                });
            });
        }

        /// <summary>
        /// Create RestEase client for specified interface using http client build with WebApplicationFactory.
        /// </summary>
        /// <typeparam name="TController">RestEase controller interface.</typeparam>
        /// <param name="client">Http client for service under test.</param>
        public static TController ClientFor<TController>(this HttpClient client) where TController : class
            => RestClient.For<TController>(client);

        private static IWebHostBuilder InitializeLogger(this IWebHostBuilder builder, ITestOutputHelper testOutputHelper)
        {
            if (testOutputHelper is null)
                return builder;

#pragma warning disable CS0618 // Type or member is obsolete
            builder.UseSerilog((_, loggerConfiguration) =>
            {
                loggerConfiguration.MinimumLevel.Is(LogEventLevel.Verbose);
                loggerConfiguration.Enrich.FromLogContext();

                loggerConfiguration.WriteTo.TestOutput(testOutputHelper,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Properties:j}{NewLine}{Exception}{NewLine}");
            });
#pragma warning restore CS0618 // Type or member is obsolete

            return builder;
        }
    }
}
