using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Repositories;
using Zoxive.HttpLoadTesting.Client.Domain.Iteration.Repositories;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Controller.Api
{
    [Route("/api")]
    public class StepResultsController : ControllerBase
    {
        private readonly IIterationResultRepository _iterationResultRepository;
        private readonly IHttpStatusResultRepository _httpStatusResultRepository;

        public StepResultsController(IIterationResultRepository iterationResultRepository, IHttpStatusResultRepository httpStatusResultRepository)
        {
            _iterationResultRepository = iterationResultRepository;
            _httpStatusResultRepository = httpStatusResultRepository;
        }

        [HttpGet("all")]
        public Task<IReadOnlyDictionary<int, UserIterationResult>> Get()
        {
            return _iterationResultRepository.GetAll();
        }

        [HttpGet("user/{id}", Name = "UserResults")]
        public Task<IEnumerable<UserIterationResult>> GetUserResults(int id)
        {
            return _iterationResultRepository.GetUserResults(id);
        }

        [HttpGet("test/{testName}", Name = "TestResults")]
        public Task<IEnumerable<UserIterationResult>> GetTestResults(string testName)
        {
            return _iterationResultRepository.GetTestResults(testName);
        }

        [HttpGet("test/names")]
        public Task<IDictionary<string, int>> GetTestNames()
        {
            return _iterationResultRepository.GetTestNames();
        }

        [HttpGet("httpStatusResult/requestUrls")]
        public Task<string[]> GetRequestUrls(string method)
        {
            return _httpStatusResultRepository.GetDistinctRequestUrls(method);
        }

        [HttpGet("httpStatusResult/methods")]
        public Task<string[]> GetMethods(string requestUrl)
        {
            return _httpStatusResultRepository.GetDistinctMethods(requestUrl);
        }

        [HttpGet("httpStatusResult/statistics")]
        public Task<HttpStatusResultStatistics> GetStatistics(string method, string requestUrl)
        {
            return _httpStatusResultRepository.GetStatistics(method, requestUrl);
        }
    }
}
