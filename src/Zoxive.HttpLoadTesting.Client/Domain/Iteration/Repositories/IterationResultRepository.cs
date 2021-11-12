using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Dtos;
using Zoxive.HttpLoadTesting.Client.Domain.Iteration.Dtos;
using Zoxive.HttpLoadTesting.Framework.Model;

#nullable disable

namespace Zoxive.HttpLoadTesting.Client.Domain.Iteration.Repositories
{
    public sealed class IterationResultRepository : IIterationResultRepository
    {
        private readonly DbConnection _dbConnection;

        public IterationResultRepository(IDbWriter dbConnection)
        {
            _dbConnection = (DbConnection)dbConnection.Connection;
        }

        private DbCommand _insertIteration;
        private DbParameter _iteration;
        private DbParameter _baseUrl;
        private DbParameter _didError;
        private DbParameter _elapsedMs;
        private DbParameter _startedMs;
        private DbParameter _exception;
        private DbParameter _testName;
        private DbParameter _userNumber;
        private DbParameter _userDelay;
        public async Task Save(UserIterationResult iterationResult)
        {
            var iterationDto = new IterationDto
            {
                Iteration = iterationResult.Iteration,
                BaseUrl = iterationResult.BaseUrl,
                DidError = iterationResult.DidError,
                StartedMs = iterationResult.StartedTime.TotalMilliseconds,
                ElapsedMs = iterationResult.Elapsed.TotalMilliseconds,
                Exception = iterationResult.Exception,
                TestName = iterationResult.TestName,
                UserNumber = iterationResult.UserNumber,
                UserDelay = iterationResult.UserDelay
            };

            const string sql = @"INSERT INTO
[Iteration] ([Iteration], [BaseUrl], [DidError], [ElapsedMs], [StartedMs], [Exception], [TestName], [UserNumber], [UserDelay])
values
(@Iteration, @BaseUrl, @DidError, @ElapsedMs, @StartedMs, @Exception, @TestName, @UserNumber, @UserDelay);
SELECT last_insert_rowid();";

            long iterationId;

            if (_insertIteration == null)
            {
                _insertIteration = _dbConnection.CreateCommand();
                _insertIteration.CommandText = sql;

                _iteration = _insertIteration.CreateParameter();
                _iteration.DbType = DbType.Int32;
                _iteration.ParameterName = "@Iteration";
                _insertIteration.Parameters.Add(_iteration);

                _baseUrl = _insertIteration.CreateParameter();
                _baseUrl.DbType = DbType.String;
                _baseUrl.ParameterName = "@BaseUrl";
                _insertIteration.Parameters.Add(_baseUrl);

                _didError = _insertIteration.CreateParameter();
                _didError.DbType = DbType.Boolean;
                _didError.ParameterName = "@DidError";
                _insertIteration.Parameters.Add(_didError);

                _elapsedMs = _insertIteration.CreateParameter();
                _elapsedMs.DbType = DbType.Double;
                _elapsedMs.ParameterName = "@ElapsedMs";
                _insertIteration.Parameters.Add(_elapsedMs);

                _startedMs = _insertIteration.CreateParameter();
                _startedMs.DbType = DbType.Double;
                _startedMs.ParameterName = "@StartedMs";
                _insertIteration.Parameters.Add(_startedMs);

                _exception = _insertIteration.CreateParameter();
                _exception.DbType = DbType.String;
                _exception.ParameterName = "@Exception";
                _insertIteration.Parameters.Add(_exception);

                _testName = _insertIteration.CreateParameter();
                _testName.DbType = DbType.String;
                _testName.ParameterName = "@TestName";
                _insertIteration.Parameters.Add(_testName);

                _userNumber = _insertIteration.CreateParameter();
                _userNumber.DbType = DbType.Int32;
                _userNumber.ParameterName = "@UserNumber";
                _insertIteration.Parameters.Add(_userNumber);

                _userDelay = _insertIteration.CreateParameter();
                _userDelay.DbType = DbType.Int64;
                _userDelay.ParameterName = "@UserDelay";
                _insertIteration.Parameters.Add(_userDelay);

                await _insertIteration.PrepareAsync();
            }

            _iteration.Value = iterationDto.Iteration;
            _baseUrl.Value = iterationDto.BaseUrl;
            _didError.Value = iterationDto.DidError;
            _elapsedMs.Value = iterationDto.ElapsedMs;
            _startedMs.Value = iterationDto.StartedMs;
            _exception.Value = iterationDto.Exception as object ?? DBNull.Value;
            _testName.Value = iterationDto.TestName;
            _userNumber.Value = iterationDto.UserNumber;
            _userDelay.Value = iterationDto.UserDelay;

            iterationId = (long)await _insertIteration.ExecuteScalarAsync();

            var inserts = iterationResult.StatusResults.Select(httpStatusResult => new HttpStatusResultDto
            {
                IterationId = (int)iterationId,
                ElapsedMilliseconds = httpStatusResult.ElapsedMilliseconds,
                Method = httpStatusResult.Method,
                RequestUrl = httpStatusResult.RequestUrl,
                StatusCode = httpStatusResult.StatusCode,
                RequestStartedMs = httpStatusResult.RequestStartedMs
            });

            await InsertResults(inserts);
        }

        private DbCommand _httpStatusResultCommand;
        private DbParameter _iterationId;
        private DbParameter _elapsedMilliseconds;
        private DbParameter _method;
        private DbParameter _requestUrl;
        private DbParameter _statusCode;
        private DbParameter _requestStartMs;

        private async Task InsertResults(IEnumerable<HttpStatusResultDto> inserts)
        {
            const string sql = @"INSERT INTO HttpStatusResult 
            ([IterationId], [ElapsedMilliseconds], [Method], [RequestUrl], [StatusCode], [RequestStartedMs])
            VALUES
            (@IterationId, @ElapsedMilliseconds, @Method, @RequestUrl, @StatusCode, @RequestStartMs)";

            if (_httpStatusResultCommand == null)
            {
                _httpStatusResultCommand = _dbConnection.CreateCommand();
                _httpStatusResultCommand.CommandText = sql;

                _iterationId = _httpStatusResultCommand.CreateParameter();
                _iterationId.DbType = DbType.Int32;
                _iterationId.ParameterName = "@IterationId";
                _httpStatusResultCommand.Parameters.Add(_iterationId);

                _elapsedMilliseconds = _httpStatusResultCommand.CreateParameter();
                _elapsedMilliseconds.DbType = DbType.Double;
                _elapsedMilliseconds.ParameterName = "@ElapsedMilliseconds";
                _httpStatusResultCommand.Parameters.Add(_elapsedMilliseconds);

                _method = _httpStatusResultCommand.CreateParameter();
                _method.DbType = DbType.String;
                _method.ParameterName = "@Method";
                _httpStatusResultCommand.Parameters.Add(_method);

                _requestUrl = _httpStatusResultCommand.CreateParameter();
                _requestUrl.DbType = DbType.String;
                _requestUrl.ParameterName = "@RequestUrl";
                _httpStatusResultCommand.Parameters.Add(_requestUrl);

                _statusCode = _httpStatusResultCommand.CreateParameter();
                _statusCode.DbType = DbType.Int32;
                _statusCode.ParameterName = "@StatusCode";
                _httpStatusResultCommand.Parameters.Add(_statusCode);

                _requestStartMs = _httpStatusResultCommand.CreateParameter();
                _requestStartMs.DbType = DbType.Double;
                _requestStartMs.ParameterName = "@RequestStartMs";
                _httpStatusResultCommand.Parameters.Add(_requestStartMs);

                await _httpStatusResultCommand.PrepareAsync();
            }

            foreach (var dto in inserts)
            {
                _iterationId.Value = dto.IterationId;
                _elapsedMilliseconds.Value = dto.ElapsedMilliseconds;
                _method.Value = dto.Method;
                _requestUrl.Value = dto.RequestUrl;
                _statusCode.Value = (int)dto.StatusCode;
                _requestStartMs.Value = dto.RequestStartedMs;

                await _httpStatusResultCommand.ExecuteNonQueryAsync();
            }
        }

        public void Dispose()
        {
            _dbConnection?.Dispose();
            _insertIteration?.Dispose();
            _httpStatusResultCommand?.Dispose();
        }
    }
}
