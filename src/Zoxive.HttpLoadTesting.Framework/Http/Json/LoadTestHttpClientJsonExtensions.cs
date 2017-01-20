using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Zoxive.HttpLoadTesting.Framework.Core;

namespace Zoxive.HttpLoadTesting.Framework.Http.Json
{
    public static class LoadTestHttpClientJsonExtensions
    {
        public static Task<HttpResponseMessage> Post(this ILoadTestHttpClient httpClient, string relativePath, HttpContent content = null)
        {
            content = content ?? EmptyContent();

            return httpClient.Post(relativePath, content);
        }

        public static Task<HttpResponseMessage> Post(this ILoadTestHttpClient httpClient, string relativePath, IDictionary<string, object> content)
        {
            return httpClient.Post(relativePath, new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"));
        }

        public static Task<HttpResponseMessage> Put(this ILoadTestHttpClient httpClient, string relativePath, IDictionary<string, object> content)
        {
            return httpClient.Put(relativePath, new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"));
        }

        private static HttpContent EmptyContent()
        {
            return new StringContent("{}", Encoding.UTF8, "application/json");
        }
    }
}