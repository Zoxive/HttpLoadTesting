using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Framework.Http
{
    public class UserTestSpecificHttpClient : IUserTestSpecificHttpClient
    {
        private ILoadTestHttpClient _loadTestHttpClient;

        private List<HttpStatusResult> _statusResults = new List<HttpStatusResult>();

        private long _userDelay;
        private Stopwatch _stopWatch;

        public UserTestSpecificHttpClient(ILoadTestHttpClient loadTestHttpClient, IDictionary<string, object> testState)
        {
            _loadTestHttpClient = loadTestHttpClient;
            TestState = testState;
            _stopWatch = new Stopwatch();
        }

        // Stores anything you need for your tests specific to this user/httpclient
        public IDictionary<string, object> TestState { get; }

        public Task<HttpResponseMessage> Post(string relativePath, HttpContent content)
        {
            return LogStatusResult(() => _loadTestHttpClient.Post(relativePath, content));
        }

        public Task<HttpResponseMessage> Put(string relativePath, HttpContent content)
        {
            return LogStatusResult(() => _loadTestHttpClient.Put(relativePath, content));
        }

        public Task<HttpResponseMessage> Get(string relativePath)
        {
            return LogStatusResult(() => _loadTestHttpClient.Get(relativePath));
        }

        public Task<HttpResponseMessage> Delete(string relativePath)
        {
            return LogStatusResult(() => _loadTestHttpClient.Delete(relativePath));
        }

        public IUserTestSpecificHttpClient GetClientForUser()
        {
            return _loadTestHttpClient.GetClientForUser();
        }

        public Task DelayUserClick(int min = 500, int max = 1000)
        {
            return LogUserDelay(() => _loadTestHttpClient.DelayUserClick(min, max));
        }

        public Task DelayUserThink(int min = 500, int max = 3000)
        {
            return LogUserDelay(() => _loadTestHttpClient.DelayUserThink(min, max));
        }

        private async Task LogUserDelay(Func<Task> func)
        {
            _stopWatch.Restart();

            await func();

            _stopWatch.Stop();

            _userDelay += _stopWatch.ElapsedMilliseconds;
        }

        public void Dispose()
        {
            _stopWatch = null;
            _loadTestHttpClient = null;
            _statusResults = null;
        }

        public IReadOnlyList<HttpStatusResult> StatusResults()
        {
            return _statusResults;
        }

        public long UserDelay()
        {
            return _userDelay;
        }

        private async Task<HttpResponseMessage> LogStatusResult(Func<Task<HttpResponseMessage>> doRequest)
        {
            _stopWatch.Restart();

            var response = await doRequest();

            _statusResults.Add(new HttpStatusResult(response, _stopWatch.ElapsedTicks));

            _stopWatch.Stop();

            return response;
        }
    }
}
