using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Framework.Core;

namespace Zoxive.HttpLoadTesting.Framework.Http.Json
{
    public static class LoadTestHttpClientJsonExtensions
    {
        public static Task<HttpResponseMessage> Post(this ILoadTestHttpClient httpClient, string relativePath, HttpContent? content = null)
        {
            content ??= EmptyContent();

            return httpClient.Post(relativePath, content);
        }

        public static Task<HttpResponseMessage> Post(this ILoadTestHttpClient httpClient, string relativePath, IDictionary<string, object> content)
        {
#pragma warning disable IDISP004
            // HttpClient disposes content
            return httpClient.Post(relativePath, JsonContent.Create(content));
#pragma warning restore IDISP004
        }

        public static Task<HttpResponseMessage> Put(this ILoadTestHttpClient httpClient, string relativePath, IDictionary<string, object> content)
        {
#pragma warning disable IDISP004
            // HttpClient disposes content
            return httpClient.Put(relativePath, JsonContent.Create(content));
#pragma warning restore IDISP004
        }

        private static HttpContent EmptyContent()
        {
            return new StringContent("{}", Encoding.UTF8, "application/json");
        }
    }
}