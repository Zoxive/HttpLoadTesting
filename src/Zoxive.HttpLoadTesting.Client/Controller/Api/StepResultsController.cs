using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Zoxive.HttpLoadTesting.Client.Domain;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Controller.Api
{
    [Route("/api")]
    public class StepResultsController : ControllerBase
    {
        private readonly IIterationResultRepository _iterationResultRepository;

        public StepResultsController(IIterationResultRepository iterationResultRepository)
        {
            _iterationResultRepository = iterationResultRepository;
        }

        [HttpGet("all")]
        public IReadOnlyDictionary<string, UserIterationResult> Get()
        {
            return _iterationResultRepository.GetAll();
        }

        [HttpGet("user/{id}", Name = "UserResults")]
        public IEnumerable<UserIterationResult> GetUserResults(int id)
        {
            return _iterationResultRepository.GetUserResults(id);
        }

        [HttpGet("test/{testName}", Name = "TestResults")]
        public IEnumerable<UserIterationResult> GetTestResults(string testName)
        {
            return _iterationResultRepository.GetTestResults(testName);
        }

        [HttpGet("test/names")]
        public IDictionary<string, int> GetTestNames()
        {
            return _iterationResultRepository.GetTestNames();
        }
    }
}
