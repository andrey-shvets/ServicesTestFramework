using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ServicesTestFramework.WebAppTools.Authentication.Options;

namespace ServicesTestFramework.WebAppTools.Authentication.Factories
{
    public static class TestAuthTokenFactory
    {
        public static string FromClaims(params Claim[] claims)
        {
            var securityTokenHandler = new JwtSecurityTokenHandler();
            var token = new JwtSecurityToken(TestJwtTokenOptions.Issuer, audience: null, claims, notBefore: null, DateTime.UtcNow.AddYears(value: 1), TestJwtTokenOptions.SigningCredentials);
            return $"{JwtBearerDefaults.AuthenticationScheme} {securityTokenHandler.WriteToken(token)}";
        }
    }
}
