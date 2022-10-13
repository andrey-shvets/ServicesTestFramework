using System.Security.Claims;
using Ardalis.Specification;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServicesTestFramework.ExampleApi.Configuration.Options;
using ServicesTestFramework.ExampleApi.Extensions;
using ServicesTestFramework.ExampleApi.Repositories;
using ServicesTestFramework.ExampleApi.Repositories.Interfaces;
using ServicesTestFramework.ExampleApi.Services;
using ServicesTestFramework.ExampleApi.Services.Interfaces;

namespace ServicesTestFramework.ExampleApi;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration) => Configuration = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvcCore()
            .AddApiExplorer();

        services.AddSwagger();

        services.AddScoped<ITestScopedService, TestScopedService>();
        services.AddSingleton<ITestSingletonService, TestSingletonService>();
        services.AddTransient<ITestTransientService, TestTransientService>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddScheme<JwtBearerOptions, JwtBearerHandler>(JwtBearerDefaults.AuthenticationScheme, options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidIssuer = "TestIssuer",
                IssuerSigningKey = new SymmetricSecurityKey(new byte[] { 0, 1, 2, 3 }) { KeyId = "TestKeyId" }
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireNameIdentifier", policy => policy.RequireClaim(ClaimTypes.NameIdentifier));
            options.AddPolicy("TestPolicy", policy => policy.RequireClaim("TestPolicy"));
            options.AddPolicy("OtherPolicy", policy => policy.RequireClaim("OtherPolicy"));
        });

        services.Configure<DatabaseOptions>(Configuration.GetSection(DatabaseOptions.SectionKey));

        services
            .AddRouting()
            .AddControllers();

        services.AddScoped<ITestDao, TestDao>();

        services.AddScoped<IMultipleImplementationsService, MultipleImplementationsService>();
        services.AddScoped<IMultipleImplementationsService, MultipleImplementationsExtraService>();

        services.AddDbContext<TestDatabaseContext>(
            (provider, options) =>
            {
                var optionsService = provider.GetRequiredService<IOptions<CosmosDbOptions>>();
                var cosmosOptions = optionsService.Value;

                options.UseCosmos(
                    cosmosOptions.AccountEndpoint,
                    cosmosOptions.AccountKey,
                    "TestDatabase",
                    cosmosOptionsAction: null);
            });

        services.AddScoped(typeof(IRepositoryBase<>), typeof(CosmosDbRepository<>));
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ExampleService v1"));

        app.UseRouting();

        app.UseAuthorization();
        app.UseAuthentication();

        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}