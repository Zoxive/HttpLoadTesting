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
            var handler = new HttpClientHandler();

            HttpUser.AlterHttpClientHandler?.Invoke(handler);

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
            return HttpClient.PostAsync(GetUrl(relativePath), content);
        }

        public Task<HttpResponseMessage> Get(string relativePath)
        {
            return HttpClient.GetAsync(GetUrl(relativePath));
        }

        public Task<HttpResponseMessage> Delete(string relativePath)
        {
            return HttpClient.DeleteAsync(GetUrl(relativePath));
        }

        public IUserTestSpecificHttpClient GetClientForUser()
        {
            return new UserTestSpecificHttpClient(this, TestState);
        }

        public Task DelayUserClick(int min = 500, int max = 1000)
        {
            return Task.Delay(Rand.Random(min, max));
        }

        public Task DelayUserThink(int min = 500, int max = 3000)
        {
            return Task.Delay(Rand.Random(min, max));
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
