using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Repositories;
using Zoxive.HttpLoadTesting.Client.Framework.Model;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IResultRepository _resultRepository;

        public IndexModel(IResultRepository resultRepository)
        {
            _resultRepository = resultRepository;
        }

        public HttpStatusResultStatistics Stats { get; private set; } =  HttpStatusResultStatistics.Empty;
        public HttpStatusResultDistincts Distincts { get; private set; } = HttpStatusResultDistincts.Empty;
        public Filters Filters { get; private set; } = Filters.Empty;

        public async Task OnGetAsync([FromQuery] Filters filters)
        {
            var distincts = _resultRepository.GetDistincts(filters);
            var stats = _resultRepository.GetStatistics(filters);

            await Task.WhenAll(distincts, stats);

            Distincts = await distincts;
            Filters = filters;
            Stats = await stats;
        }
    }

    public enum CollationType
    {
        Requests,
        Tests
    }

    public class Filters
    {
        public int Count { get; } = 50;

        public static Filters Empty => new();

        public Filters(): this(CollationType.Requests, null, null, null, null, null, null)
        {
        }

        protected Filters(CollationType collationType, string? test, string? method, string? requestUrl, int? deviations, int? statusCode, decimal? period)
        {
            CollationType = collationType;
            Test = test;
            Method = method;
            RequestUrl = requestUrl;
            Deviations = deviations;
            StatusCode = statusCode;
            Period = period ?? 1m;
        }

        public CollationType CollationType { get; }

        public string? Test { get; }

        public string? Method { get; }

        public string? RequestUrl { get; }

        public int? Deviations { get; set; }

        public int? StatusCode { get; }

        public decimal? Period { get; set; }

        public Filters NullMethod()
        {
            return new Filters(CollationType.Requests, null, null, RequestUrl, Deviations, StatusCode, Period);
        }

        public Filters NullRequestUrl()
        {
            return new Filters(CollationType.Requests, null, Method, null, Deviations, StatusCode, Period);
        }

        public Filters NullStatusCodeUrl()
        {
            return new Filters(CollationType.Requests, null, Method, RequestUrl, Deviations, null, Period);
        }
    }
}