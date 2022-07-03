using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ServicesTestFramework.WebAppTools.Extensions
{
    public static class ConfigureDbContextExtensions
    {
        /// <summary>
        /// Replaces existing <typeparamref name="TContext"/> in <see cref="IServiceCollection"/> with context
        /// configured by <paramref name="dbContextConfiguration"/>.
        /// </summary>
        public static void SwapDbContext<TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> dbContextConfiguration)
            where TContext : DbContext
        {
            var descriptor = services.FindServiceDescriptor<DbContextOptions<TContext>>();
            services.Remove(descriptor);

            services.AddDbContext<TContext>(dbContextConfiguration);
        }

        /// <summary>
        /// Replaces existing <typeparamref name="TContext"/> in <see cref="IServiceCollection"/> with <typeparamref name="TContextImplementation"/>
        /// that is configured by <paramref name="optionalDbContextConfiguration"/>.
        /// </summary>
        public static void SwapDbContext<TContext, TContextImplementation>(this IServiceCollection services, Action<DbContextOptionsBuilder> optionalDbContextConfiguration = null)
            where TContext : DbContext
            where TContextImplementation : DbContext, TContext
        {
            var descriptor = services.FindServiceDescriptor<DbContextOptions<TContext>>();
            services.Remove(descriptor);

            services.AddDbContext<TContext, TContextImplementation>(optionalDbContextConfiguration);
        }
    }
}
