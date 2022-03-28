using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ServicesTestFramework.WebAppTools.Authentication.Options;

namespace ServicesTestFramework.WebAppTools.Authentication.Factories
{
    public sealed class TestAuthTokenFactory : IAuthTokenFactory
    {
        private readonly SecurityTokenHandler _securityTokenHandler = new JwtSecurityTokenHandler();

        public static TestAuthTokenFactory Instance => new();

        public string FromClaims(params Claim[] claims)
        {
            var token = new JwtSecurityToken(TestJwtTokenOptions.Issuer, audience: null, claims, notBefore: null, DateTime.UtcNow.AddYears(value: 1), TestJwtTokenOptions.SigningCredentials);
            return $"{JwtBearerDefaults.AuthenticationScheme} {_securityTokenHandler.WriteToken(token)}";
        }
    }
}
