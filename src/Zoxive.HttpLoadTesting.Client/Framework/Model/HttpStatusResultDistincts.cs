using System.Collections.Generic;

namespace Zoxive.HttpLoadTesting.Client.Framework.Model
{
    public class HttpStatusResultDistincts
    {
        public HttpStatusResultDistincts(IEnumerable<string> methodsResult, IEnumerable<string> requestUrlUrlsResult, IEnumerable<int> statusCodes)
        {
            Methods = methodsResult;
            RequestUrls = requestUrlUrlsResult;
            StatusCodes = statusCodes;
        }

        public IEnumerable<string> RequestUrls { get; }

        public IEnumerable<string> Methods { get; }

        public IEnumerable<int> StatusCodes { get; }
    }
}
