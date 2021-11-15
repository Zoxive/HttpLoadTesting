using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Client.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Http;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Framework.Core
{
    public sealed class User : IDisposable
    {
        private readonly IReadOnlyList<ILoadTest> _loadTests;
        private readonly IHttpUser _httpUser;
        private readonly Func<TimeSpan> _getCurrentTimeSpan;
        private readonly Action<UserIterationResult> _iterationResult;
        private readonly CancellationTokenSource _cancellationToken;
        private readonly LoadTestHttpClient _loadTestHttpClient;
        private readonly ITestExecutionContextInternal _context;

        public event EventHandler? OnStop;

        public int Iteration { get; private set; }

        public int UserNumber { get; }

        public User(int userNum, IHttpUser httpUser, Func<TimeSpan> getCurrentTimeSpan, Action<UserIterationResult> iterationResult, ITestExecutionContextInternal context)
        {
            UserNumber = userNum;
            _loadTests = httpUser.Tests;
            _httpUser = httpUser;
            _getCurrentTimeSpan = getCurrentTimeSpan;
            _iterationResult = iterationResult;
            _context = context;

            _cancellationToken = new CancellationTokenSource();

            _loadTestHttpClient = new LoadTestHttpClient(httpUser, _cancellationToken.Token);

            Iteration = 0;
        }

        public async Task Initialize()
        {
            if (Initialized)
                return;

            var initializeEachTest = _loadTests
                .Select(RetryInitialize)
                .ToArray();

            try
            {
                await Task.WhenAll(initializeEachTest);
                _context.UserInitialized(this);
                Initialized = true;
            }
            catch (Exception)
            {
                Console.WriteLine($"GAVE UP Trying to initialize User {UserNumber}");
            }

        }

        public bool Initialized { get; private set; }

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

        public Task Run()
        {
            // Stop Executing
            if (_cancellationToken.IsCancellationRequested)
                return Task.CompletedTask;

            var nextTest = GetNextTest(++Iteration);

            return ExecuteTest(nextTest, _getCurrentTimeSpan);
        }

        private async Task ExecuteTest(ILoadTest nextTest, Func<TimeSpan> getCurrentTimeSpan)
        {
            var userTime = ValueStopwatch.StartNew();
            var startedTime = getCurrentTimeSpan();
            using var userSpecificClient = _loadTestHttpClient.GetClientForUser(_getCurrentTimeSpan);

            Exception? exception = null;

            try
            {
                await nextTest.Execute(userSpecificClient, _cancellationToken.Token);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            var elapsedTime = userTime.GetElapsedTime();

            var statusResults = userSpecificClient.StatusResults();
            var result = new UserIterationResult(_httpUser.BaseUrl, UserNumber, elapsedTime, Iteration, nextTest.Name, statusResults, startedTime, userSpecificClient.UserDelay, exception?.ToString());
            _iterationResult.Invoke(result);
        }

        public bool IsRunning { get; private set; } = true;

        public void Stop()
        {
            OnStop?.Invoke(this, EventArgs.Empty);

            IsRunning = false;
            _cancellationToken.Cancel();
            _loadTestHttpClient.Dispose();
        }

        private ILoadTest GetNextTest(int currentUserIdx)
        {
            var testIdx = currentUserIdx % _loadTests.Count;

            return _loadTests[testIdx];
        }

        public void Dispose()
        {
            _cancellationToken.Dispose();
            _loadTestHttpClient.Dispose();
        }
    }
}