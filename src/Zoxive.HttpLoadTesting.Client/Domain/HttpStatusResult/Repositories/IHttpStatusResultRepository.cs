using System.Collections.Generic;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Client.Framework.Model;
using Zoxive.HttpLoadTesting.Client.Pages;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Repositories
{
    public interface IHttpStatusResultRepository
    {
        Task<HttpStatusResultStatistics> GetStatistics(Filters filters);

        Task<HttpStatusResultDistincts> GetDistincts(Filters filters);

        string CreateWhereClause(Filters filters, out IDictionary<string, object> sqlParams);
    }
}
