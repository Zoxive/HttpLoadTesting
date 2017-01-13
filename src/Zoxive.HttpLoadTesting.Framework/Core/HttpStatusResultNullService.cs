using System.Collections.Generic;

namespace Zoxive.HttpLoadTesting.Framework.Core
{
    public class HttpStatusResultNullService : IHttpStatusResultService
    {
        public IEnumerable<string> SelectUniqueRequests(IEnumerable<string> allUrls)
        {
            return allUrls;
        }

        public string CreateRequestUrlWhereClause(string requestUrl, out Dictionary<string, object> sqlParams)
        {
            sqlParams = new Dictionary<string, object>
            {
                {"requestUrl", requestUrl }
            };

            return "RequestUrl = @requestUrl";
        }
    }
}
