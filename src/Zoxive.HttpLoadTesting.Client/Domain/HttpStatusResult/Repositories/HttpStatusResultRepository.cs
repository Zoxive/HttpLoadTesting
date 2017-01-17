using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Dtos;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Factories;
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

        public async Task<string[]> GetDistinctRequestUrls(string method)
        {
            var sqlParams = new Dictionary<string, object>();

            var sql = "SELECT DISTINCT RequestUrl FROM HttpStatusResult";

            if (!string.IsNullOrEmpty(method))
            {
                sql += " WHERE Method = @method";
                sqlParams.Add("method", method);
            }

            var requestUrls = await _dbConnection.QueryAsync<string>(sql, sqlParams);

            return _service.SelectUniqueRequests(requestUrls).ToArray();
        }

        public async Task<string[]> GetDistinctMethods(string requestUrl)
        {
            var sqlParams = new Dictionary<string, object>();

            var sql = "SELECT DISTINCT Method FROM HttpStatusResult";

            if (!string.IsNullOrEmpty(requestUrl))
            {
                sql += " WHERE " + _service.CreateRequestUrlWhereClause(requestUrl, out sqlParams);
            }

            var methods = await _dbConnection.QueryAsync<string>(sql, sqlParams);

            return methods.ToArray();
        }

        public async Task<HttpStatusResultStatistics> GetStatistics(string method, string requestUrl, int? deviations)
        {
            var sql = "SELECT ElapsedMilliseconds FROM HttpStatusResult";

            IDictionary<string, object> sqlParams;
            var whereClause = CreateWhereClause(method, requestUrl, out sqlParams);

            sql += whereClause;

            sql += " ORDER BY ElapsedMilliseconds DESC";

            var durations = (await _dbConnection.QueryAsync<long>(sql, sqlParams)).ToArray();

            const string slowestSql = "SELECT * FROM HttpStatusResult ORDER BY ElapsedMilliseconds DESC LIMIT 50";

            var slowestRequests = (await _dbConnection.QueryAsync<HttpStatusResultDto>(slowestSql)).ToArray();

            const string fastestSql = "SELECT * FROM HttpStatusResult ORDER BY ElapsedMilliseconds ASC LIMIT 50";

            var fastestRequests = (await _dbConnection.QueryAsync<HttpStatusResultDto>(fastestSql)).ToArray();

            return _statisticsFactory.Create(method, requestUrl, durations, deviations, slowestRequests, fastestRequests);
        }

        private string CreateWhereClause(string method, string requestUrl, out IDictionary<string, object> sqlParams)
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
                Dictionary<string, object> requestUrlSqlParams;
                var requestUrlWhereCriteria = _service.CreateRequestUrlWhereClause(requestUrl, out requestUrlSqlParams);
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

            if (whereCriteria.Count == 0)
                return "";

            var whereClause = " WHERE ";
            for(var i = 0; i < whereCriteria.Count; i++)
            {
                if (i == 0)
                {
                    whereClause += whereCriteria[i];
                }
                else
                {
                    whereClause += " AND " + whereCriteria[i];
                }
            }

            return whereClause;
        }
    }
}
