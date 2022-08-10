using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServicesTestFramework.WebAppTools.Authentication.Options;
using ServicesTestFramework.WebAppTools.Extensions;

namespace ServicesTestFramework.WebAppTools.Authentication.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Mocks authentication. Use in combination with <see cref="FakeToken"/>.
        /// </summary>
        /// <param name="services">Reference to services collection.</param>
        /// <param name="authScheme">Authentication scheme to override.</param>
        public static IServiceCollection AddMockAuthentication(this IServiceCollection services, string authScheme = JwtBearerDefaults.AuthenticationScheme)
        {
            services.Swap<IAuthenticationSchemeProvider, MockAuthenticationSchemeProvider>();

            services
                .AddAuthentication(options =>
                {
                    options.SchemeMap.Remove(authScheme);

                    options.DefaultScheme = authScheme;
                    options.DefaultAuthenticateScheme = authScheme;
                    options.DefaultChallengeScheme = authScheme;
                })
                .AddScheme<JwtBearerOptions, JwtBearerHandler>(authScheme, options => options.TokenValidationParameters = new TokenValidationParameters
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
            private static FieldInfo SchemesField =>
                typeof(MockAuthenticationSchemeProvider).BaseType!.GetField("_schemes", BindingFlags.NonPublic | BindingFlags.Instance);

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
                var schemes = SchemesField.GetValue(this) as IDictionary<string, AuthenticationScheme>;
                schemes?.Remove(scheme.Name);

                base.AddScheme(scheme);
            }
        }
    }
}
