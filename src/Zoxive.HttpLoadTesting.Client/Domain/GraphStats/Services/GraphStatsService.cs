using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Zoxive.HttpLoadTesting.Client.Domain.GraphStats.Dtos;
using Zoxive.HttpLoadTesting.Client.Domain.GraphStats.Repositories;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Repositories;
using Zoxive.HttpLoadTesting.Client.Pages;

namespace Zoxive.HttpLoadTesting.Client.Domain.GraphStats.Services
{
    public class GraphStatsService : IGraphStatsService
    {
        private readonly IDbConnection _connection;
        private readonly IRequestGraphRepository _requestGraphRepository;
        private readonly ITestGraphRepository _testGraphRepository;
        private readonly IRequestResultRepository _requestRepository;

        public GraphStatsService(
            IDbReader connection,
            IRequestGraphRepository requestGraphRepository,
            ITestGraphRepository testGraphRepository,
            IRequestResultRepository requestRepository)
        {
            _connection = connection.Connection;
            _requestGraphRepository = requestGraphRepository;
            _testGraphRepository = testGraphRepository;
            _requestRepository = requestRepository;
        }

        public async Task<IEnumerable<GraphStatDto>> Get(Filters filters)
        {
            if (!filters.Period.HasValue) throw new ArgumentNullException(nameof(filters), "Filter.Period must have a value");

            var minuteMilliseconds = Math.Round(filters.Period.Value * 60000);

            IEnumerable<GraphStatDto> result;

            switch (filters.CollationType)
            {
                case CollationType.Requests:
                    result = await _requestGraphRepository.Get(minuteMilliseconds, filters);
                    break;

                case CollationType.Tests:
                    result = await _testGraphRepository.Get(minuteMilliseconds, filters);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }

        public async Task<IEnumerable<StatusCodeStatDto>> GetStatusCodes(Filters filters)
        {
            if (!filters.Period.HasValue)
                throw new ArgumentNullException(nameof(filters), "Filter.Period must have a value");

            var minuteMilliseconds = Math.Round(filters.Period.Value * 60000);

            var httpStatusWhere = _requestRepository.CreateWhereClause(filters, out var sqlParams);

            var sql = $@"
SELECT

Minute,
COUNT(Id) as Requests,
StatusCode as StatusCode
FROM
(
    SELECT *,
    CAST(RequestStartedMs / {minuteMilliseconds} as int64) as Minute,
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
    }
}