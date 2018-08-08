using System;
using System.Collections.Generic;
using System.Data;
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
    public class HttpStatusResultRepository : IHttpStatusResultRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IHttpStatusResultStatisticsFactory _statisticsFactory;
        private readonly IHttpStatusResultService _service;

        public HttpStatusResultRepository(IDbReader dbConnection, IHttpStatusResultStatisticsFactory statisticsFactory, IHttpStatusResultService service)
        {
            _dbConnection = dbConnection.Connection;
            _statisticsFactory = statisticsFactory;
            _service = service;
        }

        private async Task<IEnumerable<string>> GetDistinctRequestUrls(Filters filters)
        {
            var sql = "SELECT DISTINCT RequestUrl FROM HttpStatusResult";

            sql += CreateWhereClause(filters.NullRequestUrl(), out var sqlParams);

            var requestUrls = await _dbConnection.QueryAsync<string>(sql, sqlParams);

            return _service.SelectUniqueRequests(requestUrls);
        }

        private Task<IEnumerable<string>> GetDistinctMethods(Filters filters)
        {
            var sql = "SELECT DISTINCT Method FROM HttpStatusResult";

            sql += CreateWhereClause(filters.NullMethod(), out var sqlParams);

            return _dbConnection.QueryAsync<string>(sql, sqlParams);
        }


        public async Task<HttpStatusResultStatistics> GetStatistics(Filters filters)
        {
            var requests = GetRequests(filters);
            var slowestRequests = GetSlowestRequests(filters);
            var fastestRequests = GetFastestRequests(filters);

            await Task.WhenAll(requests, slowestRequests, fastestRequests);

            return _statisticsFactory.Create(filters, requests.Result.AsList(), slowestRequests.Result, fastestRequests.Result);
        }

        public async Task<HttpStatusResultDistincts> GetDistincts(Filters filters)
        {
            var methods = GetDistinctMethods(filters);
            var requestUrls = GetDistinctRequestUrls(filters);
            var statusCodes = GetDistinctStatusCodes(filters);

            await Task.WhenAll(methods, requestUrls, statusCodes);

            return new HttpStatusResultDistincts(methods.Result, requestUrls.Result, statusCodes.Result);
        }

        private Task<IEnumerable<int>> GetDistinctStatusCodes(Filters filters)
        {
            var sql = "SELECT DISTINCT StatusCode FROM HttpStatusResult";

            sql += CreateWhereClause(filters.NullStatusCodeUrl(), out var sqlParams);

            return _dbConnection.QueryAsync<int>(sql, sqlParams);
        }

        private Task<IEnumerable<HttpStatusResultDto>> GetRequests(Filters filters)
        {
            var sql = "SELECT * FROM HttpStatusResult";

            var whereClause = CreateWhereClause(filters.NullRequestUrl(), out var sqlParams);

            sql += whereClause;

            sql += " ORDER BY ElapsedMilliseconds DESC";

            return _dbConnection.QueryAsync<HttpStatusResultDto>(sql, sqlParams);
        }


        private Task<IEnumerable<HttpStatusResultDto>> GetSlowestRequests(Filters filters)
        {
            var sql = "SELECT * FROM HttpStatusResult";

            var whereClause = CreateWhereClause(filters, out var sqlParams);

            sql += whereClause;

            sql += $" ORDER BY ElapsedMilliseconds DESC LIMIT {filters.Count}";

            return _dbConnection.QueryAsync<HttpStatusResultDto>(sql, sqlParams);
        }

        private Task<IEnumerable<HttpStatusResultDto>> GetFastestRequests(Filters filters)
        {
            var sql = "SELECT * FROM HttpStatusResult";

            var whereClause = CreateWhereClause(filters, out var sqlParams);

            sql += whereClause;

            sql += $" ORDER BY ElapsedMilliseconds ASC LIMIT {filters.Count}";

            return _dbConnection.QueryAsync<HttpStatusResultDto>(sql, sqlParams);
        }

        public string CreateWhereClause(Filters filters, out IDictionary<string, object> sqlParams)
        {
            sqlParams = new Dictionary<string, object>();

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
                        throw new ArgumentException(
                            string.Format(
                                "The sql parameter '{0}' provided by the CreateRequestUrlWhereClause method of the IHttpStatusResultService implementation already exists",
                                kvp.Key));
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
