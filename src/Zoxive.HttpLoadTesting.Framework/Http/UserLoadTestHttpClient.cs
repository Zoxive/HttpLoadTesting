using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Framework.Http
{
    public class UserLoadTestHttpClient : IUserLoadTestHttpClient
    {
        private ILoadTestHttpClient _loadTestHttpClient;

        private List<HttpStatusResult> _statusResults = new List<HttpStatusResult>();

        private Stopwatch _stopWatch;

        public UserLoadTestHttpClient(ILoadTestHttpClient loadTestHttpClient, IDictionary<string, object> testState)
        {
            _loadTestHttpClient = loadTestHttpClient;
            TestState = testState;
            _stopWatch = new Stopwatch();
        }

        public long UserDelay { get; private set; }

        // Stores anything you need for your tests specific to this user/httpclient
        public IDictionary<string, object> TestState { get; }

        public Task<HttpResponseMessage> Post(string relativePath, HttpContent content, IDictionary<string, string> headers = null)
        {
            return LogStatusResult(() => _loadTestHttpClient.Post(relativePath, content, headers));
        }

        public Task<HttpResponseMessage> Put(string relativePath, HttpContent content, IDictionary<string, string> headers = null)
        {
            return LogStatusResult(() => _loadTestHttpClient.Put(relativePath, content, headers));
        }

        public Task<HttpResponseMessage> Get(string relativePath, IDictionary<string, string> headers = null)
        {
            return LogStatusResult(() => _loadTestHttpClient.Get(relativePath, headers));
        }

        public Task<HttpResponseMessage> Delete(string relativePath, IDictionary<string, string> headers = null)
        {
            return LogStatusResult(() => _loadTestHttpClient.Delete(relativePath, headers));
        }

        public async Task LogUserDelay(Func<Task> func)
        {
            _stopWatch.Restart();

            await func();

            _stopWatch.Stop();

            UserDelay += _stopWatch.ElapsedMilliseconds;
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

        private async Task<HttpResponseMessage> LogStatusResult(Func<Task<HttpResponseMessage>> doRequest)
        {
            _stopWatch.Restart();
            var requestStartTick = Stopwatch.GetTimestamp();

            var response = await doRequest();

            _stopWatch.Stop();

            _statusResults.Add(new HttpStatusResult(response, _stopWatch.Elapsed.TotalMilliseconds, requestStartTick));

            return response;
        }
    }
}
