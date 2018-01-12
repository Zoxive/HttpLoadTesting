using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Framework.Http
{
    public class LoadTestHttpClient : ILoadTestHttpClient
    {
        internal readonly IHttpUser HttpUser;

        public LoadTestHttpClient(IHttpUser httpUser)
        {
            HttpUser = httpUser;
            HttpClient = CreateHttpClient();

            TestState = new Dictionary<string, object>();
        }

        private HttpClient HttpClient { get; }

        private HttpClient CreateHttpClient()
        {
            var handler = HttpUser.CreateHttpMessageHandler?.Invoke() ?? new HttpClientHandler();

            var client = new HttpClient(handler)
            {
                Timeout = new TimeSpan(0, 1, 0)
            };

            HttpUser.AlterHttpClient?.Invoke(client);

            return client;
        }

        public IDictionary<string, object> TestState { get; }

        public Task<HttpResponseMessage> Post(string relativePath, HttpContent content)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, GetUrl(relativePath))
            {
                Content = content
            };
            return SendAsync(request);
        }

        public Task<HttpResponseMessage> Put(string relativePath, HttpContent content)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, GetUrl(relativePath))
            {
                Content = content
            };
            return SendAsync(request);
        }

        public Task<HttpResponseMessage> Get(string relativePath)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, GetUrl(relativePath));

            return SendAsync(request);
        }

        public Task<HttpResponseMessage> Delete(string relativePath)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, GetUrl(relativePath));

            return SendAsync(request);
        }

        private Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            HttpUser.AlterHttpRequestMessage?.Invoke(request);

            return HttpClient.SendAsync(request);
        }

        public IUserLoadTestHttpClient GetClientForUser()
        {
            return new UserLoadTestHttpClient(this, TestState);
        }

        private Uri GetUrl(string relativePath)
        {
            return new Uri(HttpUser.BaseUrl + relativePath);
        }

        public void Dispose()
        {
            HttpClient?.Dispose();
        }
    }
}
