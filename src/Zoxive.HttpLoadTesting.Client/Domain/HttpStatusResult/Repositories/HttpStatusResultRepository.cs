using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Repositories
{
    public class HttpStatusResultRepository : IHttpStatusResultRepository
    {
        private readonly IDbConnection _dbConnection;

        public HttpStatusResultRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
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

            return requestUrls.ToArray();
        }

        public async Task<string[]> GetDistinctMethods(string requestUrl)
        {
            var sqlParams = new Dictionary<string, object>();

            var sql = "SELECT DISTINCT Method FROM HttpStatusResult";

            if (!string.IsNullOrEmpty(requestUrl))
            {
                sql += " WHERE RequestUrl = @requestUrl";
                sqlParams.Add("requestUrl", requestUrl);
            }

            var methods = await _dbConnection.QueryAsync<string>(sql, sqlParams);

            return methods.ToArray();
        }

        public async Task<HttpStatusResultStatistics> GetStatistics(string method, string requestUrl)
        {
            var sql = "SELECT ElapsedMilliseconds FROM HttpStatusResult";

            IDictionary<string, object> sqlParams;
            var whereClause = CreateWhereClause(method, requestUrl, out sqlParams);

            sql += whereClause;

            var averageDuration = await _dbConnection.ExecuteScalarAsync<long>(sql, sqlParams);

            return new HttpStatusResultStatistics(method, requestUrl, averageDuration);
        }

        private static string CreateWhereClause(string method, string requestUrl, out IDictionary<string, object> sqlParams)
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
                whereCriteria.Add("RequestUrl = @requestUrl");
                sqlParams.Add("requestUrl", requestUrl);
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
