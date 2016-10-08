using System;
using System.Collections.Generic;

namespace Zoxive.HttpLoadTesting.Framework.Model
{
    public class UserIterationResult
    {
        public IReadOnlyList<HttpStatusResult> StatusResults { get; }

        public int StartTick { get; }

        public int EndTick { get; }

        public long UserDelay { get; }

        public string Exception { get; }

        public bool DidError { get; }

        public string BaseUrl { get; }

        public int UserNumber { get; }

        public TimeSpan Elapsed { get; }

        public int Iteration { get; }

        public string TestName { get; }

        public UserIterationResult(string baseUrl, int userNumber, TimeSpan elapsed, int iteration, string testName, IReadOnlyList<HttpStatusResult> statusResults, int startTick, int endTick, long userDelay, string exception)
        {
            BaseUrl = baseUrl;
            UserNumber = userNumber;
            Elapsed = elapsed;
            Iteration = iteration;
            TestName = testName;
            StatusResults = statusResults;
            StartTick = startTick;
            EndTick = endTick;
            UserDelay = userDelay;

            DidError = exception != null;
            Exception = exception;
        }
    }
}