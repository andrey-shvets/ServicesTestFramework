using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ServicesTestFramework.WebAppTools.Extensions
{
    public static class ConfigureServicesExtensions
    {
        /// <summary>
        /// Removes all services in <see cref="IServiceCollection"/> with the same service type
        /// as <typeparamref name="TService"/> and replaces them with <paramref name="implementationInstance"/>.
        /// </summary>
        public static void Swap<TService>(this IServiceCollection services, TService implementationInstance)
            where TService : class
        {
            var descriptor = services.FindServiceDescriptor<TService>();
            services.RemoveAll<TService>();

            switch (descriptor.Lifetime)
            {
                case ServiceLifetime.Singleton:
                    services.AddSingleton<TService>(implementationInstance);
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped<TService>((_) => implementationInstance);
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient<TService>((_) => implementationInstance);
                    break;
                default:
                    throw new NotSupportedException($"Descriptor for {typeof(TService).Name} service has invalid DI lifetime {descriptor.Lifetime} ");
            }
        }

        /// <summary>
        /// Removes all services in <see cref="IServiceCollection"/> with the same service type
        /// as <typeparamref name="TService"/> and replaces them with <typeparamref name="TImplementation"/>.
        /// </summary>
        public static void Swap<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            var descriptor = services.FindServiceDescriptor<TService>();
            services.RemoveAll<TService>();

            switch (descriptor.Lifetime)
            {
                case ServiceLifetime.Singleton:
                    services.AddSingleton<TService, TImplementation>();
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped<TService, TImplementation>();
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient<TService, TImplementation>();
                    break;
                default:
                    throw new NotSupportedException($"Descriptor for {typeof(TService).Name} service has invalid DI lifetime {descriptor.Lifetime}.");
            }
        }

        /// <summary>
        /// Removes all services in <see cref="IServiceCollection"/> with the same service type
        /// as <typeparamref name="TService"/> and replaces them with <paramref name="implementationFactory"/>.
        /// </summary>
        public static void Swap<TService>(this IServiceCollection services, Func<IServiceProvider, TService> implementationFactory)
            where TService : class
        {
            var descriptor = services.FindServiceDescriptor<TService>();
            services.RemoveAll<TService>();

            switch (descriptor.Lifetime)
            {
                case ServiceLifetime.Singleton:
                    services.AddSingleton<TService>(implementationFactory);
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped<TService>(implementationFactory);
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient<TService>(implementationFactory);
                    break;
                default:
                    throw new NotSupportedException($"Descriptor for {typeof(TService).Name} service has invalid DI lifetime {descriptor.Lifetime} ");
            }
        }

        public static TService GetScopedService<TService>(this IServiceProvider serviceProvider) where TService : class
        {
            var serviceScopeFactory = serviceProvider.GetService<IServiceScopeFactory>();

            if (serviceScopeFactory is null)
                throw new Exception($"Failed to retrieve {nameof(IServiceScopeFactory)} from provided WebApplicationFactory.");

            using var serviceScope = serviceScopeFactory.CreateScope();
            return serviceScope.ServiceProvider.GetRequiredService<TService>();
        }

        private static ServiceDescriptor FindServiceDescriptor<TService>(this IServiceCollection services) where TService : class
        {
            var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(TService));

            if (descriptor is null)
                throw new InvalidOperationException($"Failed to find {typeof(TService)} service in application service collection");

            return descriptor;
        }
    }
}
