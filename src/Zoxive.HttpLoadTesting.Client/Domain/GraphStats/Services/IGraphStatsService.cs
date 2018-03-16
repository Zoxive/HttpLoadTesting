using System.Collections.Generic;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Client.Pages;

namespace Zoxive.HttpLoadTesting.Client.Domain.GraphStats.Services
{
    public interface IGraphStatsService
    {
        Task<IEnumerable<GraphStatDto>> Get(Filters filters);

        Task<IEnumerable<StatusCodeStatDto>> GetStatusCodes(Filters filters);
    }
}