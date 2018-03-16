using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly IHttpStatusResultRepository _httpStatusResultRepository;

        public LineGraphModel(IGraphStatsService graphStatsService, IHttpStatusResultRepository httpStatusResultRepository)
        {
            _graphStatsService = graphStatsService;
            _httpStatusResultRepository = httpStatusResultRepository;
        }

        public async Task OnGetAsync([FromQuery] Filters filters)
        {
            filters.Period = filters.Period ?? 1m;
            filters.Frequency = filters.Frequency ?? Stopwatch.Frequency;

            var graphStatus = _graphStatsService.Get(filters);
            var distincts = _httpStatusResultRepository.GetDistincts(filters);

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