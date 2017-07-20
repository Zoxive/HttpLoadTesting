using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zoxive.HttpLoadTesting.Client.Domain.GraphStats.Services
{
    public interface IGraphStatsService
    {
        Task<IEnumerable<GraphStatDto>> Get(int groups = 60);
    }
}