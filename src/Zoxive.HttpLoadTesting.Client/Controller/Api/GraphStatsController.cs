using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Zoxive.HttpLoadTesting.Client.Domain.GraphStats.Services;

namespace Zoxive.HttpLoadTesting.Client.Controller.Api
{
    [Route("/api/graphstats")]
    public class GraphStatsController : ControllerBase
    {
        private readonly IGraphStatsService _graphStatsService;

        public GraphStatsController(IGraphStatsService graphStatsService)
        {
            _graphStatsService = graphStatsService;
        }

        [Route("get/{groupSize}")]
        public async Task<object> Get(int groupSize = 60)
        {
            return await _graphStatsService.Get(groupSize);
        }
    }
}
