using System.Collections.Generic;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Client.Domain.GraphStats.Dtos;
using Zoxive.HttpLoadTesting.Client.Pages;

namespace Zoxive.HttpLoadTesting.Client.Domain.GraphStats.Repositories
{
    public interface IStatusCodeGraphRepository
    {
        Task<IEnumerable<StatusCodeStatDto>> Get(decimal period, Filters filters);
    }
}