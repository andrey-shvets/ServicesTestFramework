using System.Security.Claims;

namespace ServicesTestFramework.WebAppTools.Authorization.Factories
{
    public interface IAuthTokenFactory
    {
        public string FromClaims(params Claim[] claims);
    }
}
