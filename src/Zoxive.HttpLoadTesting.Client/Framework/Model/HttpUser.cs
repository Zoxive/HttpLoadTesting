using System;
using System.Collections.Generic;
using System.Net.Http;
using Zoxive.HttpLoadTesting.Framework.Core;

namespace Zoxive.HttpLoadTesting.Framework.Model
{
    public class HttpUser : IHttpUser
    {
        public string BaseUrl { get; }

        public IReadOnlyList<ILoadTest> Tests { get; }

        public Func<HttpMessageHandler>? CreateHttpMessageHandler { get; set; }

        public Action<HttpClient>? AlterHttpClient { get; set; }

        public Action<HttpRequestMessage>? AlterHttpRequestMessage { get; set; }

        public HttpUser(string baseUrl, IReadOnlyList<ILoadTest> tests)
        {
            BaseUrl = baseUrl;
            Tests = tests;
        }
    }
}