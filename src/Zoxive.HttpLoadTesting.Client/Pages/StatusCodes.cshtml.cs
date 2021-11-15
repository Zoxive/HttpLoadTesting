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
    public class StatusCodesModel : PageModel
    {
        private readonly IGraphStatsService _graphStatsService;
        private readonly IResultRepository _resultRepository;

        public StatusCodesModel(IGraphStatsService graphStatsService, IResultRepository resultRepository)
        {
            _graphStatsService = graphStatsService;
            _resultRepository = resultRepository;
        }

        public async Task OnGetAsync([FromQuery] Filters filters)
        {
            filters.Period ??= 1m;

            var graphStatus = _graphStatsService.GetStatusCodes(filters);
            var distincts = _resultRepository.GetDistincts(filters);

            await Task.WhenAll(graphStatus, distincts);

            GraphStatus = (await graphStatus).AsList();
            Distincts = await distincts;
            Filters = filters;
        }

        public HttpStatusResultDistincts? Distincts { get; set; }
        public Filters? Filters { get; set; }
        public IReadOnlyList<StatusCodeStatDto> GraphStatus { get; set; }= ArraySegment<StatusCodeStatDto>.Empty;
    }
}