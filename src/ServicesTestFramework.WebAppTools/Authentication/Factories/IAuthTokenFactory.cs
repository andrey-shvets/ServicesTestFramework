using System.Security.Claims;

namespace ServicesTestFramework.WebAppTools.Authentication.Factories
{
    public interface IAuthTokenFactory
    {
        public string FromClaims(params Claim[] claims);
    }
}
