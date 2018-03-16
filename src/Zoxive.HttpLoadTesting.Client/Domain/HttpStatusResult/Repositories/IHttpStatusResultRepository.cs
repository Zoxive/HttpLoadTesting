using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Client.Framework.Model;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Repositories
{
    public interface IHttpStatusResultRepository
    {
        Task<HttpStatusResultStatistics> GetStatistics(string method, string requestUrl, int? deviations, int? statusCode);

        Task<HttpStatusResultDistincts> GetDistincts(string method, string requestUrl, int? statusCode);
    }
}
