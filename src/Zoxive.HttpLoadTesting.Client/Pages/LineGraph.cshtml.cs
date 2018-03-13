using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Zoxive.HttpLoadTesting.Client.Domain.GraphStats.Services;

namespace Zoxive.HttpLoadTesting.Client.Pages
{
    public class LineGraphModel : PageModel
    {
        private readonly IGraphStatsService _graphStatsService;

        public LineGraphModel(IGraphStatsService graphStatsService)
        {
            _graphStatsService = graphStatsService;
        }

        public async Task OnGetAsync(decimal? period, long? frequency = null)
        {
            GroupMinutes = period ?? 1m;

            Frequency = frequency ?? Stopwatch.Frequency;

            GraphStatus = (await _graphStatsService.Get(GroupMinutes, Frequency)).AsList();
        }

        public IReadOnlyList<GraphStatDto> GraphStatus;

        public decimal GroupMinutes;
        public long Frequency;
    }
}