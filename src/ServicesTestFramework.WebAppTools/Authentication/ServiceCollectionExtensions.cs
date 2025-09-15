using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServicesTestFramework.WebAppTools.Extensions;

namespace ServicesTestFramework.WebAppTools.Authentication;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Mocks authentication. Use in combination with <see cref="FakeToken"/>.
    /// </summary>
    /// <param name="services">Reference to services collection.</param>
    public static IServiceCollection AddMockAuthentication(this IServiceCollection services)
    {
        var authScheme = JwtBearerDefaults.AuthenticationScheme;

        services.Swap<IAuthenticationSchemeProvider, MockAuthenticationSchemeProvider>();

        services.AddAuthentication(options =>
        {
            options.SchemeMap.Remove(authScheme);
            options.DefaultScheme = authScheme;
        })
        .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidIssuer = TestJwtTokenOptions.Issuer,
            IssuerSigningKey = TestJwtTokenOptions.SecurityKey
        });

        return services;
    }

    private class MockAuthenticationSchemeProvider : AuthenticationSchemeProvider
    {
        public MockAuthenticationSchemeProvider(IOptions<AuthenticationOptions> options)
            : base(options, new Dictionary<string, AuthenticationScheme>())
        {
        }

        protected MockAuthenticationSchemeProvider(IOptions<AuthenticationOptions> options, IDictionary<string, AuthenticationScheme> schemes)
            : base(options, schemes)
        {
        }

        public override void AddScheme(AuthenticationScheme scheme)
        {
            RemoveScheme(scheme.Name);
            base.AddScheme(scheme);
        }
    }
}
