using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Zoxive.HttpLoadTesting.Framework.Core
{
    public interface ILoadTestHttpClient : IDisposable
    {
        IDictionary<string, object> TestState { get; }

        Task<HttpResponseMessage> Post(string relativePath, HttpContent content);

        Task<HttpResponseMessage> Put(string relativePath, HttpContent content);

        Task<HttpResponseMessage> Get(string relativePath);

        Task<HttpResponseMessage> Delete(string relativePath);
    }
}