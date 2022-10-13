using Microsoft.AspNetCore.Mvc.Testing;

namespace ServicesTestFramework.WebAppTools.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder<TEntryPoint> WithBuilder<TEntryPoint>(this WebApplicationFactory<TEntryPoint> factory)
        where TEntryPoint : class
    {
        return factory;
    }
}