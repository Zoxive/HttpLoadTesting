using System.Collections.Generic;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Client.Domain.GraphStats.Dtos;
using Zoxive.HttpLoadTesting.Client.Pages;

namespace Zoxive.HttpLoadTesting.Client.Domain.GraphStats.Repositories
{
    public interface ITestGraphRepository
    {
        Task<IEnumerable<GraphStatDto>> Get(decimal period, Filters filters);
    }
}