﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Client.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Framework.Http
{
    public sealed class UserLoadTestHttpClient : IUserLoadTestHttpClient
    {
        private readonly ILoadTestHttpClient _loadTestHttpClient;
        private readonly Func<TimeSpan> _getCurrentTimeSpan;
        private readonly List<HttpStatusResult> _statusResults = new List<HttpStatusResult>();
        private readonly Stopwatch _stopWatch;

        public UserLoadTestHttpClient(ILoadTestHttpClient loadTestHttpClient, IDictionary<string, object> testState, Func<TimeSpan> getCurrentTimeSpan)
        {
            _loadTestHttpClient = loadTestHttpClient;
            _getCurrentTimeSpan = getCurrentTimeSpan;
            TestState = testState;
            _stopWatch = new Stopwatch();
        }

        public long UserDelay { get; private set; }

        // Stores anything you need for your tests specific to this user/httpclient
        public IDictionary<string, object> TestState { get; }

        public Task<HttpResponseMessage> Post(string relativePath, HttpContent content, Action<HttpRequestMessage>? alterHttpRequestMessagePerRequest = null)
        {
            return LogStatusResult(() => _loadTestHttpClient.Post(relativePath, content, alterHttpRequestMessagePerRequest));
        }

        public Task<HttpResponseMessage> Put(string relativePath, HttpContent content, Action<HttpRequestMessage>? alterHttpRequestMessagePerRequest = null)
        {
            return LogStatusResult(() => _loadTestHttpClient.Put(relativePath, content, alterHttpRequestMessagePerRequest));
        }

        public Task<HttpResponseMessage> Patch(string relativePath, HttpContent content, Action<HttpRequestMessage>? alterHttpRequestMessagePerRequest = null)
        {
            return LogStatusResult(() => _loadTestHttpClient.Patch(relativePath, content, alterHttpRequestMessagePerRequest));
        }

        public Task<HttpResponseMessage> Get(string relativePath, Action<HttpRequestMessage>? alterHttpRequestMessagePerRequest = null)
        {
            return LogStatusResult(() => _loadTestHttpClient.Get(relativePath, alterHttpRequestMessagePerRequest));
        }

        public Task<HttpResponseMessage> Delete(string relativePath, Action<HttpRequestMessage>? alterHttpRequestMessagePerRequest = null)
        {
            return LogStatusResult(() => _loadTestHttpClient.Delete(relativePath, alterHttpRequestMessagePerRequest));
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
        }

        public IReadOnlyList<HttpStatusResult> StatusResults()
        {
            lock (_statusResults)
            {
                return _statusResults;
            }
        }

        private async Task<HttpResponseMessage> LogStatusResult(Func<Task<HttpResponseMessage>> doRequest)
        {
            var stopWatch = ValueStopwatch.StartNew();
            var startedTime = _getCurrentTimeSpan();

            var response = await doRequest();

            var elapsed = stopWatch.GetElapsedTime();

            lock (_statusResults)
            {
                _statusResults.Add(new HttpStatusResult(response, elapsed.TotalMilliseconds, startedTime.TotalMilliseconds));
            }

            return response;
        }
    }
}
