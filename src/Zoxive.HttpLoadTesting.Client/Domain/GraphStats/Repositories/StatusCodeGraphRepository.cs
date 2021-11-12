using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Zoxive.HttpLoadTesting.Client.Domain.GraphStats.Dtos;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Repositories;
using Zoxive.HttpLoadTesting.Client.Pages;

namespace Zoxive.HttpLoadTesting.Client.Domain.GraphStats.Repositories
{
    public class StatusCodeGraphRepository : IStatusCodeGraphRepository
    {
        private readonly IRequestResultRepository _requestResultRepository;
        private readonly IDbConnection _connection;

        public StatusCodeGraphRepository(IDbReader connection, IRequestResultRepository requestResultRepository)
        {
            _requestResultRepository = requestResultRepository;
            _connection = connection.Connection;
        }

        public async Task<IEnumerable<StatusCodeStatDto>> Get(decimal period, Filters filters)
        {
            var httpStatusWhere = CreateWhereClause(filters, out var sqlParams);

            var sql = $@"
SELECT

Minute,
COUNT(Id) as Requests,
StatusCode as StatusCode
FROM
(
    SELECT *,
    CAST(RequestStartedMs / {period} as int64) as Minute,
    UserNumber
    FROM HttpStatusResult
    INNER JOIN Iteration ON Iteration.Id = HttpStatusResult.IterationId
    {httpStatusWhere}
) t
group by t.Minute, t.StatusCode

order by Minute
";

            var result = await _connection.QueryAsync<StatusCodeStatDto>(sql, sqlParams);

            return result;
        }

        private string CreateWhereClause(Filters filters, out IDictionary<string, object> sqlParams)
        {
            var httpStatusWhere = _requestResultRepository.CreateWhereClause(filters, out sqlParams);

            var hasFilterValue = !string.IsNullOrWhiteSpace(filters.Test);
            if (!hasFilterValue) return httpStatusWhere;

            sqlParams.Add("testName", filters.Test ?? string.Empty);

            return (string.IsNullOrWhiteSpace(httpStatusWhere) ? "WHERE " : httpStatusWhere + " AND ") + "TestName = @testName";
        }
    }
}