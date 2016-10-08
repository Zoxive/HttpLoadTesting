using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Zoxive.HttpLoadTesting.Client.Domain.Iteration.Dtos;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Domain.Iteration.Repositories
{
    public class IterationResultRepository : IIterationResultRepository
    {
        private readonly IDbConnection _dbConnection;

        public IterationResultRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task Save(UserIterationResult iterationResult)
        {
            var iterationDto = new IterationDto
            {
                Iteration = iterationResult.Iteration,
                BaseUrl = iterationResult.BaseUrl,
                DidError = iterationResult.DidError,
                Elapsed = iterationResult.Elapsed.Ticks,
                StartTick = iterationResult.StartTick,
                EndTick = iterationResult.EndTick,
                Exception = iterationResult.Exception,
                TestName = iterationResult.TestName,
                UserNumber = iterationResult.UserNumber,
                UserDelay = iterationResult.UserDelay
            };

            _dbConnection.Open();

            await _dbConnection.InsertAsync(iterationDto);
        }

        public async Task<IReadOnlyDictionary<int, UserIterationResult>> GetAll()
        {
            var iterations = await _dbConnection.GetAllAsync<IterationDto>();

            return iterations.ToDictionary(x => x.Id, ToModel);
        }

        public Task<IEnumerable<UserIterationResult>> GetUserResults(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserIterationResult>> GetTestResults(string testName)
        {
            throw new NotImplementedException();
        }

        public async Task<IDictionary<string, int>> GetTestNames()
        {
            var testNames = await _dbConnection.QueryAsync<TestNamesDto>("select TestName, count(Id) as Count from Iteration group by TestName");

            return testNames.ToDictionary(x => x.TestName, x => x.Count);
        }

        private static UserIterationResult ToModel(IterationDto iteration)
        {
            return new UserIterationResult(iteration.BaseUrl, iteration.UserNumber, new TimeSpan(iteration.Elapsed), iteration.Iteration, iteration.TestName, new List<HttpStatusResult>(), iteration.StartTick, iteration.EndTick, iteration.UserDelay, iteration.Exception);
        }
    }
}
