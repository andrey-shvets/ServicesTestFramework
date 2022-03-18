using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ServicesTestFramework.WebAppTools.Authorization.Options;

namespace ServicesTestFramework.WebAppTools.Authorization.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMockAuthentication(this IServiceCollection services)
        {
            const string AuthScheme = "MockAuth";

            services
                .AddAuthentication(AuthScheme)
                .AddScheme<JwtBearerOptions, JwtBearerHandler>(AuthScheme, options => options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidIssuer = TestJwtTokenOptions.Issuer,
                    IssuerSigningKey = TestJwtTokenOptions.SecurityKey
                });

            return services;
        }
    }
}
