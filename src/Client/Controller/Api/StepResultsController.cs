using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Zoxive.HttpLoadTesting.Client.Repositories;
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
            foreach (var kvp in _iterationResultRepository.GetAll())
            {
                if (kvp.Value.UserNumber == id)
                {
                    yield return kvp.Value;
                }
            }
        }

        [HttpGet("test/{testName}", Name = "TestResults")]
        public IEnumerable<UserIterationResult> GetTestResults(string testName)
        {
            foreach (var kvp in _iterationResultRepository.GetAll())
            {
                // Case sensitive atm yep
                if (kvp.Value.TestName == testName)
                {
                    yield return kvp.Value;
                }
            }
        }

        [HttpGet("test/names")]
        public IDictionary<string, int> GetTestNames()
        {
            var testNames = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var tests in _iterationResultRepository.GetAll().GroupBy(x => x.Value.TestName))
            {
                testNames.Add(tests.Key, tests.Count());
            }

            return testNames;
        }
    }
}
