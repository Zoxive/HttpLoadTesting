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

        public HttpStatusResultStatistics Stats;
        public HttpStatusResultDistincts Distincts;
        public Filters Filters { get; set; }

        public async Task OnGetAsync([FromQuery] Filters filters)
        {
            var distincts = _resultRepository.GetDistincts(filters);
            var stats = _resultRepository.GetStatistics(filters);

            await Task.WhenAll(distincts, stats);

            Distincts = distincts.Result;
            Filters = filters;
            Stats = stats.Result;
        }
    }

    public enum CollationType
    {
        Requests,
        Tests
    }

    public class Filters
    {
        public int Count = 50;

        public Filters()
        {
        }

        protected Filters(CollationType collationType, string test, string method, string requestUrl, int? deviations, int? statusCode, decimal? period)
        {
            CollationType = collationType;
            Test = test;
            Method = method;
            RequestUrl = requestUrl;
            Deviations = deviations;
            StatusCode = statusCode;
            Period = period;
        }

        public CollationType CollationType { get; set; }

        public string Test { get; set; }

        public string Method { get; set; }

        public string RequestUrl { get; set; }

        public int? Deviations { get; set; }

        public int? StatusCode { get; set; }

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