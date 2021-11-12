using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Zoxive.HttpLoadTesting.Client.Domain.GraphStats.Dtos;
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
            filters.Period ??= 1m;

            var graphStatus = _graphStatsService.Get(filters);
            var distincts = _resultRepository.GetDistincts(filters);

            await Task.WhenAll(graphStatus, distincts);

            GraphStatus = (await graphStatus).AsList();
            Distincts = await distincts;
            Filters = filters;
        }

        public HttpStatusResultDistincts? Distincts { get; private set;}
        public Filters? Filters { get; set; }
        public IReadOnlyList<GraphStatDto> GraphStatus { get; private set; } = ArraySegment<GraphStatDto>.Empty;
    }
}