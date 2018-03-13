using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Dtos;
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

            const string sql = @"INSERT INTO
[Iteration] ([Iteration], [BaseUrl], [DidError], [Elapsed], [StartTick], [EndTick], [Exception], [TestName], [UserNumber], [UserDelay])
values
(@Iteration, @BaseUrl, @DidError, @Elapsed, @StartTick, @EndTick, @Exception, @TestName, @UserNumber, @UserDelay);
SELECT last_insert_rowid();";

            _dbConnection.Open();

            try
            {
                var iterationId = await _dbConnection.ExecuteScalarAsync<int>(sql, iterationDto);

                var inserts = iterationResult.StatusResults.Select(httpStatusResult => new HttpStatusResultDto
                {
                    IterationId = iterationId,
                    ElapsedMilliseconds = httpStatusResult.ElapsedMilliseconds,
                    Method = httpStatusResult.Method,
                    RequestUrl = httpStatusResult.RequestUrl,
                    StatusCode = httpStatusResult.StatusCode,
                    RequestStartTick = httpStatusResult.RequestStartTick
                });

                await InsertHttpStatusResults(inserts, iterationResult.StatusResults.Count);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private Task InsertHttpStatusResults(IEnumerable<HttpStatusResultDto> inserts, int count)
        {
            var args = new Dictionary<string, object>();

            var stringBuilder = new StringBuilder();
            var i = 0;
            foreach (var dto in inserts)
            {
                stringBuilder.Append($"(@IterationId{i}, @ElapsedMilliseconds{i}, @Method{i}, @RequestUrl{i}, @StatusCode{i}, @RequestStartTick{i})");
                if (i < count - 1)
                    stringBuilder.AppendLine(", ");

                args[$"IterationId{i}"] = dto.IterationId;
                args[$"ElapsedMilliseconds{i}"] = dto.ElapsedMilliseconds;
                args[$"Method{i}"] = dto.Method;
                args[$"RequestUrl{i}"] = dto.RequestUrl;
                args[$"StatusCode{i}"] = dto.StatusCode;
                args[$"RequestStartTick{i}"] = dto.RequestStartTick;

                i++;
            }

            var sql = $@"INSERT INTO HttpStatusResult 
([IterationId], [ElapsedMilliseconds], [Method], [RequestUrl], [StatusCode], [RequestStartTick])
VALUES
{stringBuilder}";

            var cmd = new CommandDefinition(sql, args);

            return _dbConnection.ExecuteAsync(cmd);
        }

        public async Task<IReadOnlyDictionary<int, UserIterationResult>> GetAll()
        {
            var iterations = await _dbConnection.QueryAsync<IterationDto>("SELECT * FROM Iteration");
            var httpStatusResults = (await _dbConnection.QueryAsync<HttpStatusResultDto>("SELECT * FROM HttpStatusResult")).GroupBy(x => x.IterationId).ToDictionary(x => x.Key, y => y.AsList());

            return iterations.ToDictionary(x => x.Id, ToModel(httpStatusResults));
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

        private static Func<IterationDto, UserIterationResult> ToModel(Dictionary<int, List<HttpStatusResultDto>> httpStatusResultsDictionary)
        {
            return iteration =>
            {
                List<HttpStatusResultDto> httpStatusResultDtos;
                if (!httpStatusResultsDictionary.TryGetValue(iteration.Id, out httpStatusResultDtos))
                {
                    httpStatusResultDtos = new List<HttpStatusResultDto>();
                }

                var httpStatusResults = httpStatusResultDtos.Select(x => new HttpLoadTesting.Framework.Model.HttpStatusResult(x.Id, x.Method, x.ElapsedMilliseconds, x.RequestUrl, x.StatusCode, x.RequestStartTick));

                return new UserIterationResult(iteration.BaseUrl, iteration.UserNumber, new TimeSpan(iteration.Elapsed), iteration.Iteration, iteration.TestName, httpStatusResults.ToList(), iteration.StartTick, iteration.EndTick, iteration.UserDelay, iteration.Exception);
            };
        }
    }
}
