using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Zoxive.HttpLoadTesting.Client.Domain.GraphStats.Dtos;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Repositories;
using Zoxive.HttpLoadTesting.Client.Pages;

namespace Zoxive.HttpLoadTesting.Client.Domain.GraphStats.Repositories
{
    public class RequestGraphRepository : IRequestGraphRepository
    {
        private readonly IDbConnection _connection;
        private readonly IRequestResultRepository _requestResultRepository;

        public RequestGraphRepository(IDbReader connection, IRequestResultRepository requestResultRepository)
        {
            _connection = connection.Connection;
            _requestResultRepository = requestResultRepository;
        }

        public async Task<IEnumerable<GraphStatDto>> Get(decimal period, Filters filters)
        {
            var httpStatusWhere = _requestResultRepository.CreateWhereClause(filters, out var sqlParams);

            var sql = $@"
SELECT

Minute,
COUNT(Id) as Requests,
COUNT(DISTINCT UserNumber) as Users,
AVG(ElapsedMilliseconds) as Avg,
MIN(ElapsedMilliseconds) as Min,
MAX(ElapsedMilliseconds) as Max,
SUM(ElapsedMilliseconds * ElapsedMilliseconds) / COUNT(Id) - AVG(ElapsedMilliseconds) * AVG(ElapsedMilliseconds) AS Variance
FROM
(
    SELECT *,
    CAST(RequestStartedMs / {period} as int64) as Minute,
    UserNumber
    FROM HttpStatusResult
    INNER JOIN Iteration ON Iteration.Id = HttpStatusResult.IterationId
    {httpStatusWhere}
) t
group by t.Minute

order by Minute
";

            var result = await _connection.QueryAsync<GraphStatDto>(sql, sqlParams);

            return result;
        }
    }
}