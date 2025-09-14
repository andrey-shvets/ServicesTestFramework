namespace ServicesTestFramework.ExampleApi.Configuration.Options;

public class JwtAuthenticationOptions
{
    public const string SectionKey = "JwtAuthentication";

    public string SecurityKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}
