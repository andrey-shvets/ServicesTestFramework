namespace ServicesTestFramework.ExampleApi.Configuration.Options;

public class DatabaseOptions
{
    public const string SectionKey = "Database";

    public string ConnectionString { get; init; }
}