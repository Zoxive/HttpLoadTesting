using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Repositories;
using Zoxive.HttpLoadTesting.Client.Pages;

namespace Zoxive.HttpLoadTesting.Client.Domain.GraphStats.Services
{
    public class GraphStatsService : IGraphStatsService
    {
        private readonly IDbConnection _connection;
        private readonly IHttpStatusResultRepository _httpStatusRepository;

        public GraphStatsService(IDbReader connection, IHttpStatusResultRepository httpStatusRepository)
        {
            _connection = connection.Connection;
            _httpStatusRepository = httpStatusRepository;
        }

        public async Task<IEnumerable<GraphStatDto>> Get(Filters filters)
        {
            if (!filters.Period.HasValue)
                throw new ArgumentNullException(nameof(filters), "Filter.Period must have a value");
            if (!filters.Frequency.HasValue)
                throw new ArgumentNullException(nameof(filters), "Filter.Frequence must have a value");

            var minuteMilliseconds = Math.Round(filters.Period.Value * 60000);

            var frequency = filters.Frequency;

            var httpStatusWhere = _httpStatusRepository.CreateWhereClause(filters, out var sqlParams);

            var min = await _connection.QueryFirstAsync<long>($@"
SELECT
MIN(RequestStartTick) as Min
FROM HttpStatusResult");

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
    (RequestStartTick - {min}) / ({frequency} / 1000) AS MsFromStart,
    ((RequestStartTick - {min}) / ({frequency} / 1000)) / {minuteMilliseconds} as Minute,
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

        public async Task<IEnumerable<StatusCodeStatDto>> GetStatusCodes(Filters filters)
        {
            if (!filters.Period.HasValue)
                throw new ArgumentNullException(nameof(filters), "Filter.Period must have a value");
            if (!filters.Frequency.HasValue)
                throw new ArgumentNullException(nameof(filters), "Filter.Frequence must have a value");

            var minuteMilliseconds = Math.Round(filters.Period.Value * 60000);

            var frequency = filters.Frequency;

            var httpStatusWhere = _httpStatusRepository.CreateWhereClause(filters, out var sqlParams);

            var min = await _connection.QueryFirstAsync<long>($@"
SELECT
MIN(RequestStartTick) as Min
FROM HttpStatusResult");

            var sql = $@"
SELECT 

Minute,
COUNT(Id) as Requests,
StatusCode as StatusCode
FROM
(
    SELECT *,
    (RequestStartTick - {min}) / ({frequency} / 1000) AS MsFromStart,
    ((RequestStartTick - {min}) / ({frequency} / 1000)) / {minuteMilliseconds} as Minute,
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

    public class GraphStatDto
    {
        public int Minute { get; set; }

        public int Requests { get; set; }

        public int Users { get; set; }

        public decimal Avg { get; set; }

        public decimal Min { get; set; }

        public decimal Max { get; set; }

        public double Variance { get; set; }

        public double Std => Math.Sqrt(Variance);
    }

    public class StatusCodeStatDto
    {
        public int Minute { get; set; }

        public int Requests { get; set; }

        public int StatusCode { get; set; }
    }
    }
