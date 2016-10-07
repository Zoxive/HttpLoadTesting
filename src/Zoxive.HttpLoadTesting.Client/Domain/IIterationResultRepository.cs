using System.Collections.Generic;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Domain
{
    public interface IIterationResultRepository
    {
        void Save(UserIterationResult iterationResult);

        IReadOnlyDictionary<string, UserIterationResult> GetAll();

        IEnumerable<UserIterationResult> GetUserResults(int id);

        IEnumerable<UserIterationResult> GetTestResults(string testName);

        IDictionary<string, int> GetTestNames();
    }
}