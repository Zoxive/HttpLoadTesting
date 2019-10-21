using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Zoxive.HttpLoadTesting.Framework.Core
{
    public interface ILoadTestHttpClient : IDisposable
    {
        IDictionary<string, object> TestState { get; }

        Task<HttpResponseMessage> Post(string relativePath, HttpContent content, Action<HttpRequestMessage> alterHttpRequestMessagePerRequest = null);

        Task<HttpResponseMessage> Put(string relativePath, HttpContent content, Action<HttpRequestMessage> alterHttpRequestMessagePerRequest = null);

        Task<HttpResponseMessage> Patch(string relativePath, HttpContent content, Action<HttpRequestMessage> alterHttpRequestMessagePerRequest = null);

        Task<HttpResponseMessage> Get(string relativePath, Action<HttpRequestMessage> alterHttpRequestMessagePerRequest = null);

        Task<HttpResponseMessage> Delete(string relativePath, Action<HttpRequestMessage> alterHttpRequestMessagePerRequest = null);
    }
}