using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Dtos;
using Zoxive.HttpLoadTesting.Client.Pages;

namespace Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Repositories
{
    public class TestResultRepository : ITestResultRepository
    {
        private const string Sql = @"SELECT
 HttpStatusResult.[IterationId] as Id
,HttpStatusResult.IterationId
,HttpStatusResult.IterationId as Method
,SUM(HttpStatusResult.[ElapsedMilliseconds]) AS ElapsedMilliseconds
,TestName as RequestUrl
,200 as StatusCode
,MIN(HttpStatusResult.[RequestStartedMs]) AS [RequestStartedMs]
FROM HttpStatusResult
INNER JOIN Iteration ON Iteration.Id = HttpStatusResult.IterationId
";

        private readonly IDbConnection _dbConnection;

        public TestResultRepository(IDbReader dbConnection)
        {
            _dbConnection = dbConnection.Connection;
        }

        public async Task<IReadOnlyCollection<HttpStatusResultDto>> GetTests(Filters filters)
        {
            var sql = Sql;

            var whereClause = CreateWhereClause(filters, out var sqlParams);

            sql += whereClause;

            sql += " GROUP BY Iteration.Id";

            sql += " ORDER BY ElapsedMilliseconds DESC";

            var results = await _dbConnection.QueryAsync<HttpStatusResultDto>(sql, sqlParams);

            return results.ToList();
        }

        public Task<IEnumerable<HttpStatusResultDto>> GetSlowestTests(Filters filters)
        {
            var sql = Sql;

            var whereClause = CreateWhereClause(filters, out var sqlParams);

            sql += whereClause;

            sql += " GROUP BY Iteration.Id";

            sql += $" ORDER BY ElapsedMilliseconds DESC LIMIT {filters.Count}";

            return _dbConnection.QueryAsync<HttpStatusResultDto>(sql, sqlParams);
        }

        public Task<IEnumerable<HttpStatusResultDto>> GetFastestTests(Filters filters)
        {
            var sql = Sql;

            var whereClause = CreateWhereClause(filters, out var sqlParams);

            sql += whereClause;

            sql += " GROUP BY Iteration.Id";

            sql += $" ORDER BY ElapsedMilliseconds ASC LIMIT {filters.Count}";

            return _dbConnection.QueryAsync<HttpStatusResultDto>(sql, sqlParams);
        }

        public string CreateWhereClause(Filters filters, out IDictionary<string, object> sqlParams)
        {
            sqlParams = new Dictionary<string, object>();

            var hasFilterValue = !string.IsNullOrWhiteSpace(filters.Test);
            if (!hasFilterValue) return "";

            sqlParams.Add("testName", filters.Test);

            return "WHERE TestName = @testName";
        }
    }
}