using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ServicesTestFramework.WebAppTools.Authentication;

internal class TestJwtAuthenticationHandler : AuthenticationHandler<JwtBearerOptions>
{
    public TestJwtAuthenticationHandler(IOptionsMonitor<JwtBearerOptions> options,
                                        ILoggerFactory logger,
                                        UrlEncoder encoder)
        : base(options, logger, encoder)
    { }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authHeader = Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.Ordinal))
            return AuthenticateResult.NoResult();

        var token = authHeader.Substring("Bearer ".Length).Trim();

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var validationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = TestJwtTokenOptions.Issuer,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = TestJwtTokenOptions.SecurityKey
            };

            var validationResult = await handler.ValidateTokenAsync(token, validationParams);
            var principal = new ClaimsPrincipal(validationResult.ClaimsIdentity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
        catch (SecurityTokenException ex)
        {
            return AuthenticateResult.Fail($"Invalid test token: {ex.Message}");
        }
    }
}
