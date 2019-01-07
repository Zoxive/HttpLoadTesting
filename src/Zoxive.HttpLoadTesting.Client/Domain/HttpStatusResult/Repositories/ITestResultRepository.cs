using System.Collections.Generic;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Dtos;
using Zoxive.HttpLoadTesting.Client.Pages;

namespace Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Repositories
{
    public interface ITestResultRepository
    {
        Task<IReadOnlyCollection<HttpStatusResultDto>> GetTests(Filters filters);

        Task<IEnumerable<HttpStatusResultDto>> GetSlowestTests(Filters filters);

        Task<IEnumerable<HttpStatusResultDto>> GetFastestTests(Filters filters);

        string CreateWhereClause(Filters filters, out IDictionary<string, object> sqlParams);
    }
}