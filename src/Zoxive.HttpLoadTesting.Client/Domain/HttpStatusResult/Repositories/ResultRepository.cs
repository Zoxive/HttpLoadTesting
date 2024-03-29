﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Dtos;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Factories;
using Zoxive.HttpLoadTesting.Client.Framework.Model;
using Zoxive.HttpLoadTesting.Client.Pages;
using Zoxive.HttpLoadTesting.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Repositories
{
    public class ResultRepository : IResultRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IHttpStatusResultStatisticsFactory _statisticsFactory;
        private readonly IHttpStatusResultService _service;
        private readonly IRequestResultRepository _requestResultRepository;
        private readonly ITestResultRepository _testResultRepository;

        public ResultRepository(
            IDbReader dbConnection,
            IHttpStatusResultStatisticsFactory statisticsFactory,
            IHttpStatusResultService service,
            IRequestResultRepository requestResultRepository,
            ITestResultRepository testResultRepository)
        {
            _dbConnection = dbConnection.Connection;
            _statisticsFactory = statisticsFactory;
            _service = service;
            _requestResultRepository = requestResultRepository;
            _testResultRepository = testResultRepository;
        }

        private async Task<IEnumerable<string>> GetDistinctRequestUrls(Filters filters)
        {
            var sql = "SELECT DISTINCT RequestUrl FROM HttpStatusResult";

            sql += CreateWhereClause(filters.NullRequestUrl(), out var sqlParams);

            var requestUrls = await _dbConnection.QueryAsync<string>(sql, sqlParams);

            return _service.SelectUniqueRequests(requestUrls);
        }

        private Task<IEnumerable<string>> GetDistinctTests()
        {
            const string sql = "SELECT DISTINCT TestName FROM Iteration";

            return _dbConnection.QueryAsync<string>(sql);
        }

        private Task<IEnumerable<string>> GetDistinctMethods(Filters filters)
        {
            var sql = "SELECT DISTINCT Method FROM HttpStatusResult";

            sql += CreateWhereClause(filters.NullMethod(), out var sqlParams);

            return _dbConnection.QueryAsync<string>(sql, sqlParams);
        }

        public async Task<HttpStatusResultStatistics> GetStatistics(Filters filters)
        {
            Task<IEnumerable<HttpStatusResultDto>> results;
            Task<IEnumerable<HttpStatusResultDto>> slowestRequests;
            Task<IEnumerable<HttpStatusResultDto>> fastestRequests;

            switch (filters.CollationType)
            {
                case CollationType.Requests:
                    results = _requestResultRepository.GetRequests(filters);
                    slowestRequests = _requestResultRepository.GetSlowestRequests(filters);
                    fastestRequests = _requestResultRepository.GetFastestRequests(filters);
                    break;

                case CollationType.Tests:
                    results = _testResultRepository.GetTests(filters);
                    slowestRequests = _testResultRepository.GetSlowestTests(filters);
                    fastestRequests = _testResultRepository.GetFastestTests(filters);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(filters.CollationType), filters.CollationType, "CollationType unknown");
            }

            await Task.WhenAll(results, slowestRequests, fastestRequests);

            return _statisticsFactory.Create(filters, (await results).ToList(), await slowestRequests, await fastestRequests);
        }

        public async Task<HttpStatusResultDistincts> GetDistincts(Filters filters)
        {
            var tests = GetDistinctTests();
            var methods = GetDistinctMethods(filters);
            var requestUrls = GetDistinctRequestUrls(filters);
            var statusCodes = GetDistinctStatusCodes(filters);

            await Task.WhenAll(tests, methods, requestUrls, statusCodes);

            return new HttpStatusResultDistincts(await tests, await methods, await requestUrls, await statusCodes);
        }

        private Task<IEnumerable<int>> GetDistinctStatusCodes(Filters filters)
        {
            var sql = "SELECT DISTINCT StatusCode FROM HttpStatusResult";

            sql += CreateWhereClause(filters.NullStatusCodeUrl(), out var sqlParams);

            return _dbConnection.QueryAsync<int>(sql, sqlParams);
        }

        private string CreateWhereClause(Filters filters, out IDictionary<string, object> sqlParams)
        {
            sqlParams = new Dictionary<string, object>();

            if (filters.CollationType == CollationType.Tests)
            {
                var filterByTest = !string.IsNullOrWhiteSpace(filters.Test);
                var whereClause = filterByTest ? "WHERE TestName = @testName" : "";
                if (filterByTest)
                {
                    sqlParams.Add("testName", filters.Test ?? string.Empty);
                }

                return whereClause;
            }

            var whereCriteria = new List<string>();
            if (!string.IsNullOrEmpty(filters.Method))
            {
                whereCriteria.Add("Method = @method");
                sqlParams.Add("method", filters.Method);
            }

            if (!string.IsNullOrEmpty(filters.RequestUrl))
            {
                var requestUrlWhereCriteria = _service.CreateRequestUrlWhereClause(filters.RequestUrl, out var requestUrlSqlParams);
                whereCriteria.Add(requestUrlWhereCriteria);

                foreach (var kvp in requestUrlSqlParams)
                {
                    if (sqlParams.ContainsKey(kvp.Key))
                    {
                        throw new ArgumentException($"The sql parameter '{kvp.Key}' provided by the CreateRequestUrlWhereClause method of the IHttpStatusResultService implementation already exists");
                    }

                    sqlParams.Add(kvp.Key, kvp.Value);
                }
            }

            if (filters.StatusCode.HasValue)
            {
                whereCriteria.Add("StatusCode = @statusCode");
                sqlParams.Add("statusCode", filters.StatusCode.Value);
            }

            if (whereCriteria.Count == 0)
                return "";

            return " WHERE " + string.Join(" AND ", whereCriteria);
        }
    }
}