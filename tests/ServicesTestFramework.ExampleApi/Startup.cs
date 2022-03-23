using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ServicesTestFramework.ExampleApi.Configuration.Options;
using ServicesTestFramework.ExampleApi.Extensions;
using ServicesTestFramework.ExampleApi.Repositories;
using ServicesTestFramework.ExampleApi.Repositories.Interfaces;
using ServicesTestFramework.ExampleApi.Services;
using ServicesTestFramework.ExampleApi.Services.Interfaces;

namespace ServicesTestFramework.ExampleApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

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

            services.Configure<DatabaseOptions>(Configuration.GetSection("Database"));

            services
                .AddRouting()
                .AddControllers();

            services.AddScoped<ITestDao, TestDao>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
}
