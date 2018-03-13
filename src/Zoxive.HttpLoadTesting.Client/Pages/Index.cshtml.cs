using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Repositories;
using Zoxive.HttpLoadTesting.Client.Domain.Iteration.Repositories;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IIterationResultRepository _iterationResultRepository;
        private readonly IHttpStatusResultRepository _httpStatusResultRepository;

        public IndexModel(IIterationResultRepository iterationResultRepository, IHttpStatusResultRepository httpStatusResultRepository)
        {
            _iterationResultRepository = iterationResultRepository;
            _httpStatusResultRepository = httpStatusResultRepository;
        }

        public HttpStatusResultStatistics Stats;
        public IEnumerable<string> Methods;
        public IEnumerable<string> RequestUrls;

        public async Task OnGetAsync(string method = null, string requestUrl = null, int? deviations = null)
        {
            var requestUrls = _httpStatusResultRepository.GetDistinctRequestUrls(method);
            var methods = _httpStatusResultRepository.GetDistinctMethods(requestUrl);
            var stats = _httpStatusResultRepository.GetStatistics(method, requestUrl, deviations);

            await Task.WhenAll(requestUrls, methods, stats);

            RequestUrls = requestUrls.Result;
            Methods = methods.Result;
            Stats = stats.Result;
        }
    }
}