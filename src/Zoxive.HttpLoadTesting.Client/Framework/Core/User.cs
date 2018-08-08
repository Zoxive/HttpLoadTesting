using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Client.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Http;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Framework.Core
{
    public class User
    {
        private readonly IReadOnlyList<ILoadTest> _loadTests;
        private readonly IHttpUser _httpUser;
        private readonly CancellationTokenSource _cancellationToken;
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

            Iteration = 0;
        }

        public async Task Initialize()
        {
            var initializeEachTest = _loadTests
                .Select(RetryInitialize)
                .ToArray();

            await Task.WhenAll(initializeEachTest);
        }

        private async Task RetryInitialize(ILoadTest test)
        {
            var count = 0;
            while (true)
            {
                try
                {
                    await test.Initialize(_loadTestHttpClient);
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Initialize User {UserNumber} Failed. for Test {test.Name}");
                    Console.WriteLine(e);
                    count++;

                    if (count > 1)
                    {
                        Console.WriteLine("User will not be added.");
                        throw;
                    }

                    Console.WriteLine("Trying again.");
                }
            }
        }

        public async Task Run(Func<TimeSpan> getCurrentTimeSpan, Action<UserIterationResult> iterationResult)
        {
            // Stop Executing
            if (_cancellationToken.IsCancellationRequested)
                return;

            var nextTest = GetNextTest(++Iteration);

            var userSpecificClient = _loadTestHttpClient.GetClientForUser(getCurrentTimeSpan);

            var task = ExecuteTest(nextTest, userSpecificClient, getCurrentTimeSpan);

            await task.ContinueWith((task1, o) => 
            {
                iterationResult(task.Result);

                userSpecificClient.Dispose();

                return Run(getCurrentTimeSpan, iterationResult);
            }, null, _cancellationToken.Token);
        }

        private async Task<UserIterationResult> ExecuteTest(ILoadTest nextTest, IUserLoadTestHttpClient userLoadClient, Func<TimeSpan> getCurrentTimeSpan)
        {
            var userTime = ValueStopwatch.StartNew();
            var startedTime = getCurrentTimeSpan();

            Exception exception = null;

            try
            {
                await nextTest.Execute(userLoadClient);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            var elapsedTime = userTime.GetElapsedTime();

            var statusResults = userLoadClient.StatusResults();
            return new UserIterationResult(_httpUser.BaseUrl, UserNumber, elapsedTime, Iteration, nextTest.Name, statusResults, startedTime, userLoadClient.UserDelay, exception?.ToString());
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