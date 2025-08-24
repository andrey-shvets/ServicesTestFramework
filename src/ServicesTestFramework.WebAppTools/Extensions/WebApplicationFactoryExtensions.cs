using System.Text.Json;
using RestEase;

namespace ServicesTestFramework.WebAppTools.Extensions;

public static class WebApplicationFactoryExtensions
{
    /// <summary>
    /// Create RestEase client for specified interface using http client build with WebApplicationFactory.
    /// </summary>
    /// <typeparam name="TController">RestEase controller interface.</typeparam>
    /// <param name="client">Http client for service under test.</param>
    public static TController ClientFor<TController>(this HttpClient client) where TController : class
        => RestClient.For<TController>(client);

    private class JsonBodySerializer : RequestBodySerializer
    {
        public override HttpContent SerializeBody<T>(T body, RequestBodySerializerInfo info)
        {
            if (body == null)
                return null;

            var jsonBody = JsonSerializer.Serialize(body);

            var content = new StringContent(jsonBody)
            {
                Headers = { ContentType = { MediaType = "application/json" } }
            };

            return content;
        }
    }
}
