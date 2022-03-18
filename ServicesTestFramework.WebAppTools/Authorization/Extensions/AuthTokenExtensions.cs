using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using ServicesTestFramework.WebAppTools.Authorization.Factories;

namespace ServicesTestFramework.WebAppTools.Authorization.Extensions
{
    public static class AuthTokenExtensions
    {
        public static string FromSubjectId(this IAuthTokenFactory authTokenFactory, Guid? id = null, params string[] policyClaims)
        {
            var claims = new List<Claim> { new Claim(JwtRegisteredClaimNames.Sub, (id ?? Guid.NewGuid()).ToString()) };
            claims.AddRange(policyClaims.Select(claim => new Claim(claim, string.Empty)));

            return authTokenFactory.FromClaims(claims.ToArray());
        }
    }
}
