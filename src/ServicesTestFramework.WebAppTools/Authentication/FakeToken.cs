using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ServicesTestFramework.WebAppTools.Authentication.Options;

namespace ServicesTestFramework.WebAppTools.Authentication
{
    public class FakeToken
    {
        private IList<Claim> Claims { get; } = new List<Claim>();

        private FakeToken(Claim claim)
        {
            Claims.Add(claim);
        }

        public static FakeToken WithClaim(Claim claim) => new FakeToken(claim);

        public static FakeToken WithClaim(string claimType, string claimValue) => new FakeToken(new Claim(claimType, claimValue));

        public static FakeToken WithClaim(string claimType) => WithClaim(claimType, string.Empty);

        public static FakeToken WithJwtId(Guid id) => new FakeToken(new Claim(JwtRegisteredClaimNames.Sub, id.ToString()));

        public static FakeToken WithJwtId() => WithJwtId(Guid.NewGuid());

        public FakeToken AndClaim(Claim claim)
        {
            Claims.Add(claim);

            return this;
        }

        public FakeToken AndClaim(string claimType, string claimValue)
        {
            Claims.Add(new Claim(claimType, claimValue));

            return this;
        }

        public FakeToken AndClaim(string claimType)
        {
            return AndClaim(claimType, string.Empty);
        }

        public FakeToken AndJwtId(Guid id)
        {
            Claims.Add(new Claim(JwtRegisteredClaimNames.Sub, id.ToString()));

            return this;
        }

        public FakeToken AndJwtId()
        {
            return AndJwtId(Guid.NewGuid());
        }

        public FakeToken And(Claim claim)
        {
            return AndClaim(claim);
        }

        public FakeToken And(string claimType)
        {
            return AndClaim(claimType);
        }

        public FakeToken And(string claimType, string claimValue)
        {
            return AndClaim(claimType, claimValue);
        }

        public static implicit operator Claim[](FakeToken token) => token.Claims.ToArray();

        public static implicit operator string(FakeToken token) => token.ToString();

        public override string ToString() => FromClaims(Claims.ToArray());

        public static string FromClaims(params Claim[] claims)
        {
            var securityTokenHandler = new JwtSecurityTokenHandler();
            var token = new JwtSecurityToken(TestJwtTokenOptions.Issuer, audience: null, claims, notBefore: null, DateTime.UtcNow.AddYears(value: 1), TestJwtTokenOptions.SigningCredentials);
            return $"{JwtBearerDefaults.AuthenticationScheme} {securityTokenHandler.WriteToken(token)}";
        }
    }
}
