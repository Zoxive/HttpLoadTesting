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

        public User(int userNum, IReadOnlyList<ILoadTest> loadTests, IHttpUser httpUser)
        {
            UserNumber = userNum;
            _loadTests = loadTests;
            _httpUser = httpUser;

            _loadTestHttpClient = new LoadTestHttpClient(httpUser);

            _cancellationToken = new CancellationTokenSource();
            _userTime = new Stopwatch();
            Iteration = 0;
        }

        public async Task Initialize()
        {
            var initializeEachTest = _loadTests
                .Select(test => test.Initialize(_loadTestHttpClient))
                .ToArray();

            await Task.WhenAll(initializeEachTest);
        }

        public async Task Run(Action<UserIterationResult> iterationResult)
        {
            var nextTest = GetNextTest(++Iteration);

            var userSpecificClient = _loadTestHttpClient.GetClientForUser();

            var task = ExecuteTest(nextTest, userSpecificClient);

            await task.ContinueWith<Task>((task1, o) => 
            {
                iterationResult(task.Result);

                userSpecificClient.Dispose();

                return Run(iterationResult);
            }, null, _cancellationToken.Token);
        }

        private async Task<UserIterationResult> ExecuteTest(ILoadTest nextTest, IUserTestSpecificHttpClient userSpecificClient)
        {
            _userTime.Restart();
            var startTick = Env.Tick;

            Exception exception = null;

            try
            {
                await nextTest.Execute(userSpecificClient);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            _userTime.Stop();
            var endTick = Env.Tick;

            return new UserIterationResult(_httpUser.BaseUrl, UserNumber, _userTime.Elapsed, Iteration, nextTest.Name, userSpecificClient.StatusResults(), startTick, endTick, userSpecificClient.UserDelay(), exception);
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