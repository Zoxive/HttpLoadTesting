using System;
using System.Collections.Generic;
using Zoxive.HttpLoadTesting.Client.Domain.Database.Dtos;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Domain.Database.Repositories
{
    public class IterationResultRepository : IIterationResultRepository
    {
        public void Save(UserIterationResult iterationResult)
        {
            var iterationDto = new IterationDto
            {
                Iteration = iterationResult.Iteration,
                BaseUrl = iterationResult.BaseUrl,
                DidError = iterationResult.DidError,
                Elapsed = iterationResult.Elapsed,
                StartTick = iterationResult.StartTick,
                EndTick = iterationResult.EndTick,
                Exception = iterationResult.Exception?.ToString(),
                TestName = iterationResult.TestName,
                UserNumber = iterationResult.UserNumber,
                UserDelay = iterationResult.UserDelay
            };
        }

        public IReadOnlyDictionary<string, UserIterationResult> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserIterationResult> GetUserResults(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserIterationResult> GetTestResults(string testName)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, int> GetTestNames()
        {
            throw new NotImplementedException();
        }
    }
}
