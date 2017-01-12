using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Repositories
{
    public interface IHttpStatusResultRepository
    {
        Task<string[]> GetDistinctRequestUrls(string method);
        Task<string[]> GetDistinctMethods(string requestUrl);
        Task<HttpStatusResultStatistics> GetStatistics(string method, string requestUrl);
    }
}
