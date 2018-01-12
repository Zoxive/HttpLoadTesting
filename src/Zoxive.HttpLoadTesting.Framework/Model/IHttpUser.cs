using System;
using System.Collections.Generic;
using System.Net.Http;
using Zoxive.HttpLoadTesting.Framework.Core;

namespace Zoxive.HttpLoadTesting.Framework.Model
{
    public interface IHttpUser
    {
        string BaseUrl { get; }

        IReadOnlyList<ILoadTest> Tests { get; }

        Func<HttpMessageHandler> CreateHttpMessageHandler { get; set; }

        Action<HttpClient> AlterHttpClient { get; set; }

        Action<HttpRequestMessage> AlterHttpRequestMessage { get; set; }
    }
}
