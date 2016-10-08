using System.Collections.Generic;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Domain.Iteration.Repositories
{
    public interface IIterationResultRepository
    {
        Task Save(UserIterationResult iterationResult);

        Task<IReadOnlyDictionary<int, UserIterationResult>> GetAll();

        Task<IEnumerable<UserIterationResult>> GetUserResults(int id);

        Task<IEnumerable<UserIterationResult>> GetTestResults(string testName);

        Task<IDictionary<string, int>> GetTestNames();
    }
}