using System;
using System.Linq;
using System.Collections.Generic;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Domain.InMemory.Repositories
{
    public class InMemoryIterationResultRepository : IIterationResultRepository
    {
        private readonly IDictionary<string, UserIterationResult> _results = new Dictionary<string, UserIterationResult>(StringComparer.OrdinalIgnoreCase);

        public void Save(UserIterationResult iterationResult)
        {
            var key = string.Join(".", iterationResult.UserNumber, iterationResult.Iteration);

            _results.Add(key, iterationResult);
        }

        public IReadOnlyDictionary<string, UserIterationResult> GetAll()
        {
            return (IReadOnlyDictionary<string, UserIterationResult>)_results;
        }

        public IEnumerable<UserIterationResult> GetUserResults(int id)
        {
            foreach (var kvp in _results)
            {
                if (kvp.Value.UserNumber == id)
                {
                    yield return kvp.Value;
                }
            }
        }

        public IEnumerable<UserIterationResult> GetTestResults(string testName)
        {
            foreach (var kvp in _results)
            {
                // Case sensitive atm yep
                if (kvp.Value.TestName == testName)
                {
                    yield return kvp.Value;
                }
            }
        }

        public IDictionary<string, int> GetTestNames()
        {
            var testNames = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var tests in _results)
            {
                var testName = tests.Value.TestName;

                int currentCount;
                if (testNames.TryGetValue(testName, out currentCount))
                {
                    currentCount++;
                }
                else
                {
                    currentCount = 1;
                }

                testNames[testName] = currentCount;
            }

            return testNames;
        }
    }
}
