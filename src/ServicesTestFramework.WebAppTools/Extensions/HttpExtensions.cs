using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IDXOrchestrator.Extensions
{
    public static class HttpExtensions
    {
        public static string ToJson(this object obj) =>
            JsonConvert.SerializeObject(obj);

        public static T ReadAs<T>(this string json) =>
            JsonConvert.DeserializeObject<T>(json);

        public static async Task<T> ReadAs<T>(this HttpContent content)
        {
            var json = await content.ReadAsStringAsync();

            return json.ReadAs<T>();
        }

        public static StringContent ToHttpContent(this object obj)
        {
            var json = obj.ToJson();

            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        public static HttpResponseMessage ToHttpResponseMessage(this object contentObject, HttpStatusCode statusCode) =>
            new HttpResponseMessage(statusCode) { Content = contentObject.ToHttpContent() };

        public static HttpResponseMessage ToOKResponseMessage(this object contentObject) =>
            contentObject.ToHttpResponseMessage(HttpStatusCode.OK);

        public static HttpResponseMessage ToBadRequestResponseMessage(this object contentObject) =>
            contentObject.ToHttpResponseMessage(HttpStatusCode.BadRequest);
    }
}
