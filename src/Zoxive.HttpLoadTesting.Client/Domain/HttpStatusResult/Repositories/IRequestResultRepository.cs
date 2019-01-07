using System.Collections.Generic;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Dtos;
using Zoxive.HttpLoadTesting.Client.Pages;

namespace Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Repositories
{
    public interface IRequestResultRepository
    {
        Task<IEnumerable<HttpStatusResultDto>> GetRequests(Filters filters);

        Task<IEnumerable<HttpStatusResultDto>> GetSlowestRequests(Filters filters);

        Task<IEnumerable<HttpStatusResultDto>> GetFastestRequests(Filters filters);

        string CreateWhereClause(Filters filters, out IDictionary<string, object> sqlParams);
    }
}