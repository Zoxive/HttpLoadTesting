using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace Zoxive.HttpLoadTesting.Client.Domain.GraphStats.Services
{
    public class GraphStatsService : IGraphStatsService
    {
        private readonly IDbConnection _connection;

        public GraphStatsService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<GraphStatDto>> Get(int groups = 60)
        {
            var groupSize = await _connection.QueryFirstAsync<GraphStatsGroupDto>($@"
SELECT
MIN(RequestStartTick) as Min,
ROUND(CAST(MAX(RequestStartTick) - MIN(RequestStartTick) as float) / {groups}.0) as Size  FROM HttpStatusResult
");

            var sql = $@"
SELECT 

Grp,
COUNT(Id) as Requests,
COUNT(DISTINCT UserNumber) as Users,
AVG(ElapsedMilliseconds) as Avg,
MIN(ElapsedMilliseconds) as Min,
MAX(ElapsedMilliseconds) as Max,
SUM(ElapsedMilliseconds * ElapsedMilliseconds) / COUNT(Id) - AVG(ElapsedMilliseconds) * AVG(ElapsedMilliseconds) AS Variance

FROM
(
    SELECT *,
    CAST((RequestStartTick - {groupSize.Min}) as Integer) / {groupSize.Size} as Grp,
    UserNumber
    FROM HttpStatusResult

    INNER JOIN Iteration ON Iteration.Id = HttpStatusResult.IterationId
) t
group by t.Grp

order by Grp
";

            var result = await _connection.QueryAsync<GraphStatDto>(sql);

            return result;
        }
    }

    public class GraphStatsGroupDto
    {
        public long Min { get; set; }

        public long Size { get; set; }
    }

    public class GraphStatDto
    {
        public int Grp { get; set; }

        public int Requests { get; set; }

        public int Users { get; set; }

        public decimal Avg { get; set; }

        public decimal Min { get; set; }

        public decimal Max { get; set; }

        public double Variance { get; set; }

        public double Std => Math.Sqrt(Variance);
    }
}
