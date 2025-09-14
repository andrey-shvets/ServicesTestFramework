using System.Security.Claims;
using System.Text;
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
        services.Configure<JwtAuthenticationOptions>(Configuration.GetSection(JwtAuthenticationOptions.SectionKey));

        var jwtAuthentication = new JwtAuthenticationOptions();
        Configuration.GetSection(JwtAuthenticationOptions.SectionKey).Bind(jwtAuthentication);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtAuthentication.Issuer,
                    ValidAudience = jwtAuthentication.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuthentication.SecurityKey))
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireNameIdentifier", policy => policy.RequireClaim(ClaimTypes.NameIdentifier));
            options.AddPolicy("TestPolicy", policy => policy.RequireClaim("TestPolicy"));
            options.AddPolicy("OtherPolicy", policy => policy.RequireClaim("OtherPolicy"));
        });

        services.AddControllers();
        services.AddEndpointsApiExplorer();

        services.AddSwagger();

        services.AddScoped<ITestScopedService, TestScopedService>();
        services.AddSingleton<ITestSingletonService, TestSingletonService>();
        services.AddTransient<ITestTransientService, TestTransientService>();

        services.Configure<DatabaseOptions>(Configuration.GetSection(DatabaseOptions.SectionKey));

        services.AddScoped<ITestDao, TestDao>();

        services.AddScoped<IMultipleImplementationsService, MultipleImplementationsService>();
        services.AddScoped<IMultipleImplementationsService, MultipleImplementationsExtraService>();

        services.AddScoped(typeof(IRepositoryBase<>), typeof(CosmosDbRepository<>));

        services.Configure<CosmosDbOptions>(Configuration.GetSection(CosmosDbOptions.SectionKey));

        services.AddDbContext<TestDatabaseContext>((provider, options) =>
        {
            var optionsService = provider.GetRequiredService<IOptions<CosmosDbOptions>>();
            var cosmosOptions = optionsService.Value;

            options.UseCosmos(
                cosmosOptions.AccountEndpoint,
                cosmosOptions.AccountKey,
                cosmosOptions.DatabaseName);
        });
    }

    public void Configure(IApplicationBuilder builder)
    {
        builder.UseDeveloperExceptionPage();
        builder.UseSwagger();
        builder.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "ExampleService API v1");
            c.RoutePrefix = string.Empty;
        });

        builder.UseRouting();

        builder.UseAuthentication();
        builder.UseAuthorization();

        builder.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
