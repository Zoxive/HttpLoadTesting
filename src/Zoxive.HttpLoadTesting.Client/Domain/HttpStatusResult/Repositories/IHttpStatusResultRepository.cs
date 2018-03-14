using System.Collections.Generic;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Repositories
{
    public interface IHttpStatusResultRepository
    {
        Task<IEnumerable<string>> GetDistinctRequestUrls(string method);
        Task<IEnumerable<string>> GetDistinctMethods(string requestUrl);
        Task<HttpStatusResultStatistics> GetStatistics(string method, string requestUrl, int? deviations);
    }
}
