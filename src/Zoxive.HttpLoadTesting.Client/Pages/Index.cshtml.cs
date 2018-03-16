using System.Threading.Tasks;
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

        public async Task OnGetAsync(string method = null, string requestUrl = null, int? deviations = null, int? statusCode = null)
        {
            var distincts = _httpStatusResultRepository.GetDistincts(method, requestUrl, statusCode);
            var stats = _httpStatusResultRepository.GetStatistics(method, requestUrl, deviations, statusCode);

            await Task.WhenAll(distincts, stats);

            Distincts = distincts.Result;
            Stats = stats.Result;
        }
    }
}