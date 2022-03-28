using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ServicesTestFramework.WebAppTools.Authentication.Options;

namespace ServicesTestFramework.WebAppTools.Authentication.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMockAuthentication(this IServiceCollection services)
        {
            const string AuthScheme = "MockAuth";

            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = AuthScheme;
                    options.DefaultAuthenticateScheme = AuthScheme;
                    options.DefaultChallengeScheme = AuthScheme;
                })
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
