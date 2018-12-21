using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Zoxive.HttpLoadTesting.Client.Domain.GraphStats.Services;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Repositories;
using Zoxive.HttpLoadTesting.Client.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Pages
{
    public class LineGraphModel : PageModel
    {
        private readonly IGraphStatsService _graphStatsService;
        private readonly IResultRepository _resultRepository;

        public LineGraphModel(IGraphStatsService graphStatsService, IResultRepository resultRepository)
        {
            _graphStatsService = graphStatsService;
            _resultRepository = resultRepository;
        }

        public async Task OnGetAsync([FromQuery] Filters filters)
        {
            filters.Period = filters.Period ?? 1m;

            var graphStatus = _graphStatsService.Get(filters);
            var distincts = _resultRepository.GetDistincts(filters);

            await Task.WhenAll(graphStatus, distincts);

            GraphStatus = graphStatus.Result.AsList();
            Distincts = distincts.Result;
            Filters = filters;
        }

        public HttpStatusResultDistincts Distincts;
        public Filters Filters { get; set; }
        public IReadOnlyList<GraphStatDto> GraphStatus;
    }
}