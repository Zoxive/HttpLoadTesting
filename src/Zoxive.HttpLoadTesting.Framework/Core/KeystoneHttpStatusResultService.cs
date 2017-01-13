using System.Collections.Generic;
using System.Linq;

namespace Zoxive.HttpLoadTesting.Framework.Core
{
    // TODO: move this outside of here once I can reference the latest in KeystoneLoadTests
    public class KeystoneHttpStatusResultService : IHttpStatusResultService
    {
        private const string ApiBaseUrl = "https://ksdev.pcm.infor.com/runtimeapi/EnterpriseQuoter/Entities/";

        public IEnumerable<string> SelectUniqueRequests(IEnumerable<string> allUrls)
        {
            return allUrls.Select(GetEntityName).Distinct();
        }

        public string CreateRequestUrlWhereClause(string requestUrl, out Dictionary<string, object> sqlParams)
        {
            sqlParams = new Dictionary<string, object>
            {
                {"requestUrlSingle", "%/" + requestUrl + "(____________________________________)"},
                {"requestUrlEnd", "%/" + requestUrl }
            };

            return "(RequestUrl like @requestUrlSingle OR RequestUrl like @requestUrlEnd)";
        }

        private static string GetEntityName(string url)
        {
            // quote(...)/ExpandAllLines
            // quote(...)
            // quote
            // quote(...)/quoteline

            var startIndex = url.LastIndexOf('/') + 1;
            var action = url.Substring(startIndex);
            var startIndexOfId = action.IndexOf('(');
            if (startIndexOfId > 0)
            {
                action = action.Substring(0, startIndexOfId);
            }

            return action;
        }
    }
}
