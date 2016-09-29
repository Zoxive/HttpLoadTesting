using System;
using System.Collections.Generic;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Repositories
{
    public class IterationResultRepository : IIterationResultRepository
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
    }
}
