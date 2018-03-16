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
        private readonly IHttpStatusResultRepository _httpStatusResultRepository;

        public IndexModel(IHttpStatusResultRepository httpStatusResultRepository)
        {
            _httpStatusResultRepository = httpStatusResultRepository;
        }

        public HttpStatusResultStatistics Stats;
        public HttpStatusResultDistincts Distincts;

        public async Task OnGetAsync([FromQuery] Filters filters)
        {
            var distincts = _httpStatusResultRepository.GetDistincts(filters);
            var stats = _httpStatusResultRepository.GetStatistics(filters);

            await Task.WhenAll(distincts, stats);

            Distincts = distincts.Result;
            Stats = stats.Result;
        }
    }

    public class Filters
    {
        public Filters()
        {
            
        }

        protected Filters(string method, string requestUrl, int? deviations, int? statusCode)
        {
            Method = method;
            RequestUrl = requestUrl;
            Deviations = deviations;
            StatusCode = statusCode;
        }

        public string Method { get; set; }

        public string RequestUrl { get; set; }

        public int? Deviations { get; set; }

        public int? StatusCode { get; set;}

        public int Count = 50;

        public Filters NullMethod()
        {
            return new Filters(null, RequestUrl, Deviations, StatusCode);
        }

        public Filters NullRequestUrl()
        {
            return new Filters(Method, null, Deviations, StatusCode);
        }

        public Filters NullStatusCodeUrl()
        {
            return new Filters(Method, RequestUrl, Deviations, null);
        }
    }
}