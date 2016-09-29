using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Zoxive.HttpLoadTesting.Framework.Core
{
    public interface ILoadTestExecution
    {
        event UserIterationFinished UserIterationFinished;

        Task Execute(IReadOnlyList<ISchedule> schedule, CancellationToken? token = null);
    }
}