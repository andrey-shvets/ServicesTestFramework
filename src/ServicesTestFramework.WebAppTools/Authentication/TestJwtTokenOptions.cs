using System.Linq;
using Microsoft.IdentityModel.Tokens;

namespace ServicesTestFramework.WebAppTools.Authentication.Options;

internal static class TestJwtTokenOptions
{
    private const int SecurityKeyBytesCount = 32;

    public static string Issuer { get; } = nameof(TestJwtTokenOptions);
    public static SecurityKey SecurityKey { get; }

    public static SigningCredentials SigningCredentials { get; }

    static TestJwtTokenOptions()
    {
        var key = Enumerable.Repeat(byte.MinValue, SecurityKeyBytesCount).ToArray();
        SecurityKey = new SymmetricSecurityKey(key) { KeyId = "TestKeyId" };
        SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
    }
}