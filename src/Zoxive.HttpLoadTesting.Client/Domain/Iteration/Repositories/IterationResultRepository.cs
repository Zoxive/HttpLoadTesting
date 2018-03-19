using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Dtos;
using Zoxive.HttpLoadTesting.Client.Domain.Iteration.Dtos;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Domain.Iteration.Repositories
{
    public class IterationResultRepository : IIterationResultRepository
    {
        private readonly Microsoft.Data.Sqlite.SqliteConnection _dbConnection;

        private readonly object _obj = new object();

        public IterationResultRepository(IDbWriter dbConnection)
        {
            _dbConnection = (Microsoft.Data.Sqlite.SqliteConnection)dbConnection.Connection;
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

            if (_dbConnection.State != ConnectionState.Open)
                await _dbConnection.OpenAsync();

            using (MonitorLock.CreateLock(_obj, _dbConnection.ConnectionString))
            {
                await RawExecuteAsync("BEGIN TRANSACTION");
                try
                {
                    var cmd = new CommandDefinition(sql, iterationDto);

                    var iterationId = await _dbConnection.ExecuteScalarAsync<int>(cmd);

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
                        await InsertHttpStatusResults(batch, batch.Count);
                    }

                    await RawExecuteAsync("COMMIT TRANSACTION");
                }
                catch (Exception)
                {
                    await RawExecuteAsync("ROLLBACK TRANSACTION");
                    throw;
                }
            }
        }

        private async Task RawExecuteAsync(string sql)
        {
            using (var cmd = _dbConnection.CreateCommand())
            {
                cmd.CommandText = sql;

                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task InsertHttpStatusResults(IEnumerable<HttpStatusResultDto> inserts, int count)
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

            await _dbConnection.ExecuteAsync(cmd);
        }
    }

    public class MonitorLock : IDisposable
    {
        public static MonitorLock CreateLock(object value, string name)
        {
            return new MonitorLock(value, name);
        }

        private readonly object _l;
        private readonly string _name;

        protected MonitorLock(object l, string name)
        {
            _l = l;
            _name = name;

            if (!Monitor.TryEnter(_l, 100))
            {
                Console.WriteLine($"Lock {_l} attempt by {name} and failed");
            }
        }

        public void Dispose()
        {
            Monitor.Exit(_l);
        }
    }
}
