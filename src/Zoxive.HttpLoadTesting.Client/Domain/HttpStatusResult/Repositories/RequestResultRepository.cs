using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Dtos;
using Zoxive.HttpLoadTesting.Client.Pages;
using Zoxive.HttpLoadTesting.Framework.Core;

namespace Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Repositories
{
    public class RequestResultRepository : IRequestResultRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IHttpStatusResultService _service;

        public RequestResultRepository(IDbReader dbConnection, IHttpStatusResultService service)
        {
            _dbConnection = dbConnection.Connection;
            _service = service;
        }

        public async Task<IReadOnlyCollection<HttpStatusResultDto>> GetRequests(Filters filters)
        {
            var sql = "SELECT * FROM HttpStatusResult";

            var whereClause = CreateWhereClause(filters.NullRequestUrl(), out var sqlParams);

            sql += whereClause;

            sql += " ORDER BY ElapsedMilliseconds DESC";

            var results = await _dbConnection.QueryAsync<HttpStatusResultDto>(sql, sqlParams);

            return results.ToList();
        }

        public Task<IEnumerable<HttpStatusResultDto>> GetSlowestRequests(Filters filters)
        {
            var sql = "SELECT * FROM HttpStatusResult";

            var whereClause = CreateWhereClause(filters, out var sqlParams);

            sql += whereClause;

            sql += $" ORDER BY ElapsedMilliseconds DESC LIMIT {filters.Count}";

            return _dbConnection.QueryAsync<HttpStatusResultDto>(sql, sqlParams);
        }

        public Task<IEnumerable<HttpStatusResultDto>> GetFastestRequests(Filters filters)
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