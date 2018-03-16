using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Dtos;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Factories;
using Zoxive.HttpLoadTesting.Client.Framework.Model;
using Zoxive.HttpLoadTesting.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Repositories
{
    public class HttpStatusResultRepository : IHttpStatusResultRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IHttpStatusResultStatisticsFactory _statisticsFactory;
        private readonly IHttpStatusResultService _service;

        public HttpStatusResultRepository(IDbConnection dbConnection, IHttpStatusResultStatisticsFactory statisticsFactory, IHttpStatusResultService service)
        {
            _dbConnection = dbConnection;
            _statisticsFactory = statisticsFactory;
            _service = service;
        }

        private async Task<IEnumerable<string>> GetDistinctRequestUrls(string method, int? statusCode)
        {
            var sql = "SELECT DISTINCT RequestUrl FROM HttpStatusResult";

            sql += CreateWhereClause(method, null, statusCode, out var sqlParams);

            var requestUrls = await _dbConnection.QueryAsync<string>(sql, sqlParams);

            return _service.SelectUniqueRequests(requestUrls);
        }

        private Task<IEnumerable<string>> GetDistinctMethods(string requestUrl, int? statusCode)
        {
            var sql = "SELECT DISTINCT Method FROM HttpStatusResult";

            sql += CreateWhereClause(null, requestUrl, statusCode, out var sqlParams);

            return _dbConnection.QueryAsync<string>(sql, sqlParams);
        }

        public async Task<HttpStatusResultStatistics> GetStatistics(string method, string requestUrl, int? deviations, int? statusCode)
        {
            const int count = 50;

            var requests = GetRequests(method, requestUrl, statusCode);
            var slowestRequests = GetSlowestRequests(method, requestUrl, statusCode, count);
            var fastestRequests = GetFastestRequests(method, requestUrl, statusCode, count);

            await Task.WhenAll(requests, slowestRequests, fastestRequests);

            return _statisticsFactory.Create(method, requestUrl, statusCode, requests.Result, deviations, slowestRequests.Result, fastestRequests.Result);
        }

        public async Task<HttpStatusResultDistincts> GetDistincts(string method, string requestUrl, int? statusCode)
        {
            var methods = GetDistinctMethods(requestUrl, statusCode);
            var requestUrls = GetDistinctRequestUrls(method, statusCode);
            var statusCodes = GetDistinctStatusCodes(method, requestUrl);

            await Task.WhenAll(methods, requestUrls, statusCodes);

            return new HttpStatusResultDistincts(methods.Result, requestUrls.Result, statusCodes.Result);
        }

        private Task<IEnumerable<int>> GetDistinctStatusCodes(string method, string requestUrl)
        {
            var sql = "SELECT DISTINCT StatusCode FROM HttpStatusResult";

            sql += CreateWhereClause(method, requestUrl, null, out var sqlParams);

            return _dbConnection.QueryAsync<int>(sql, sqlParams);
        }

        private Task<IEnumerable<HttpStatusResultDto>> GetRequests(string method, string requestUrl, int? statusCode)
        {
            var sql = "SELECT * FROM HttpStatusResult";

            var whereClause = CreateWhereClause(method, requestUrl, statusCode, out var sqlParams);

            sql += whereClause;

            sql += " ORDER BY ElapsedMilliseconds DESC";

            return _dbConnection.QueryAsync<HttpStatusResultDto>(sql, sqlParams);
        }

        private Task<IEnumerable<HttpStatusResultDto>> GetSlowestRequests(string method, string requestUrl, int? statusCode, int count)
        {
            var sql = "SELECT * FROM HttpStatusResult";

            var whereClause = CreateWhereClause(method, requestUrl, statusCode, out var sqlParams);

            sql += whereClause;

            sql += string.Format(" ORDER BY ElapsedMilliseconds DESC LIMIT {0}", count);

            return _dbConnection.QueryAsync<HttpStatusResultDto>(sql, sqlParams);
        }

        private Task<IEnumerable<HttpStatusResultDto>> GetFastestRequests(string method, string requestUrl, int? statusCode, int count)
        {
            var sql = "SELECT * FROM HttpStatusResult";

            var whereClause = CreateWhereClause(method, requestUrl, statusCode, out var sqlParams);

            sql += whereClause;

            sql += string.Format(" ORDER BY ElapsedMilliseconds ASC LIMIT {0}", count);

            return _dbConnection.QueryAsync<HttpStatusResultDto>(sql, sqlParams);
        }

        private string CreateWhereClause(string method, string requestUrl, int? statusCode, out IDictionary<string, object> sqlParams)
        {
            sqlParams = new Dictionary<string, object>();

            var whereCriteria = new List<string>();
            if (!string.IsNullOrEmpty(method))
            {
                whereCriteria.Add("Method = @method");
                sqlParams.Add("method", method);
            }

            if (!string.IsNullOrEmpty(requestUrl))
            {
                var requestUrlWhereCriteria = _service.CreateRequestUrlWhereClause(requestUrl, out var requestUrlSqlParams);
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

            if (statusCode.HasValue)
            {
                whereCriteria.Add("StatusCode = @statusCode");
                sqlParams.Add("statusCode", statusCode.Value);
            }

            if (whereCriteria.Count == 0)
                return "";

            return " WHERE " + string.Join(" AND ", whereCriteria);
        }
    }
}
