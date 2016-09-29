using System;
using System.Net.Http;

namespace Zoxive.HttpLoadTesting.Framework.Model
{
    public interface IHttpUser
    {
        string BaseUrl { get; }

        Action<HttpClientHandler> AlterHttpClientHandler { get; set; }

        Action<HttpClient> AlterHttpClient { get; set; }
    }
}
