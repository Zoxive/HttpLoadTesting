using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Repositories
{
    public class HttpStatusResultRepository : IHttpStatusResultRepository
    {
        private readonly IDbConnection _dbConnection;

        public HttpStatusResultRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string[]> GetDistinctRequestUrls(string method)
        {
            var requestUrls =
                await
                    _dbConnection.QueryAsync<string>(
                        "SELECT DISTINCT RequestUrl FROM HttpStatusResult WHERE Method = @method", new {method});

            return requestUrls.ToArray();
        }

        public async Task<string[]> GetDistinctMethods(string requestUrl)
        {
            var methods =
                await
                    _dbConnection.QueryAsync<string>(
                        "SELECT DISTINCT Method FROM HttpStatusResult WHERE RequestUrl = @requestUrl", new {requestUrl});

            return methods.ToArray();
        }

        public async Task<HttpStatusResultStatistics> GetStatistics(string method, string requestUrl)
        {
            var averageDuration =
                await
                    _dbConnection.ExecuteScalarAsync<long>(
                        "SELECT AVG(ElapsedMilliseconds) AS AverageDuration FROM HttpStatusResult");

            return new HttpStatusResultStatistics(method, requestUrl, averageDuration);
        }
    }
}
