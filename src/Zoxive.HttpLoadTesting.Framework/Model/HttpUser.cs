using System;
using System.Net.Http;

namespace Zoxive.HttpLoadTesting.Framework.Model
{
    public class HttpUser : IHttpUser
    {
        public string BaseUrl { get; }

        public Action<HttpClientHandler> AlterHttpClientHandler { get; set; }

        public Action<HttpClient> AlterHttpClient { get; set; }

        public Action<HttpRequestMessage> AlterHttpRequestMessage { get; set; }

        public HttpUser(string baseUrl)
        {
            BaseUrl = baseUrl;
        }
    }
}