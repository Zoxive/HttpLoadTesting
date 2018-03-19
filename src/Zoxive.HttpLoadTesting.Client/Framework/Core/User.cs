using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Framework.Http;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Framework.Core
{
    public class User
    {
        private readonly IReadOnlyList<ILoadTest> _loadTests;
        private readonly IHttpUser _httpUser;
        private readonly CancellationTokenSource _cancellationToken;
        private readonly Stopwatch _userTime;
        private readonly LoadTestHttpClient _loadTestHttpClient;

        public int Iteration { get; private set; }

        public int UserNumber { get; }

        public User(int userNum, IHttpUser httpUser)
        {
            UserNumber = userNum;
            _loadTests = httpUser.Tests;
            _httpUser = httpUser;

            _cancellationToken = new CancellationTokenSource();

            _loadTestHttpClient = new LoadTestHttpClient(httpUser, _cancellationToken.Token);

            _userTime = new Stopwatch();
            Iteration = 0;
        }

        public async Task Initialize()
        {
            var initializeEachTest = _loadTests
                .Select(RetryInitialize)
                .ToArray();

            await Task.WhenAll(initializeEachTest);
        }

        private Task RetryInitialize(ILoadTest test)
        {
            var count = 0;
            while (true)
            {
                try
                {
                    return test.Initialize(_loadTestHttpClient);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Initialize User FAILED. for Test {test.Name}");
                    Console.WriteLine(e);
                    count++;

                    if (count > 1)
                        throw;
                }
            }
        }

        public async Task Run(Action<UserIterationResult> iterationResult)
        {
            // Stop Executing
            if (_cancellationToken.IsCancellationRequested)
                return;

            var nextTest = GetNextTest(++Iteration);

            var userSpecificClient = _loadTestHttpClient.GetClientForUser();

            var task = ExecuteTest(nextTest, userSpecificClient);

            await task.ContinueWith((task1, o) => 
            {
                iterationResult(task.Result);

                userSpecificClient.Dispose();

                return Run(iterationResult);
            }, null, _cancellationToken.Token);
        }

        private async Task<UserIterationResult> ExecuteTest(ILoadTest nextTest, IUserLoadTestHttpClient userLoadClient)
        {
            _userTime.Restart();
            var startTick = Stopwatch.GetTimestamp();

            Exception exception = null;

            try
            {
                await nextTest.Execute(userLoadClient);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            _userTime.Stop();

            var endTick = startTick + _userTime.ElapsedTicks;

            var statusResults = userLoadClient.StatusResults();
            return new UserIterationResult(_httpUser.BaseUrl, UserNumber, _userTime.Elapsed, Iteration, nextTest.Name, statusResults, startTick, endTick, userLoadClient.UserDelay, exception?.ToString());
        }

        public void Stop()
        {
            _cancellationToken.Cancel();
            _loadTestHttpClient.Dispose();
        }

        private ILoadTest GetNextTest(int currentUserIdx)
        {
            var testIdx = currentUserIdx % _loadTests.Count;

            return _loadTests[testIdx];
        }
    }
}