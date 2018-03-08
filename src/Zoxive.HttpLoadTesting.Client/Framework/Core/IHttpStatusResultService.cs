using System.Collections.Generic;

namespace Zoxive.HttpLoadTesting.Framework.Core
{
    public interface IHttpStatusResultService
    {
        IEnumerable<string> SelectUniqueRequests(IEnumerable<string> allUrls);

        string CreateRequestUrlWhereClause(string requestUrl, out Dictionary<string, object> sqlParams);
    }
}
