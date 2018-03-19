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

        public IterationResultRepository(IDbWriter dbConnection)
        {
            _dbConnection = dbConnection.Connection;
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

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    var iterationId = await _dbConnection.ExecuteScalarAsync<int>(sql, iterationDto, transaction);

                    var inserts = iterationResult.StatusResults.Select(httpStatusResult => new HttpStatusResultDto
                    {
                        IterationId = iterationId,
                        ElapsedMilliseconds = httpStatusResult.ElapsedMilliseconds,
                        Method = httpStatusResult.Method,
                        RequestUrl = httpStatusResult.RequestUrl,
                        StatusCode = httpStatusResult.StatusCode,
                        RequestStartTick = httpStatusResult.RequestStartTick
                    });

                    foreach (var batch in inserts.Batch(100))
                    {
                        await InsertHttpStatusResults(batch, batch.Count, transaction);
                    }

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction?.Rollback();
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        private Task InsertHttpStatusResults(IEnumerable<HttpStatusResultDto> inserts, int count, IDbTransaction transaction)
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

            var cmd = new CommandDefinition(sql, args, transaction);

            return _dbConnection.ExecuteAsync(cmd);
        }
    }
}
