namespace ServicesTestFramework.ExampleApi.Configuration.Options;

public class CosmosDbOptions
{
    public const string SectionKey = "CosmosDb";

    public string AccountEndpoint { get; set; } = string.Empty;
    public string AccountKey { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public bool IsLocal => AccountEndpoint.Contains("localhost");
}
