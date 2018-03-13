using System.Net;

namespace Zoxive.HttpLoadTesting.Framework.Model
{
    public class HttpStatusCodeCount
    {
        public HttpStatusCodeCount(HttpStatusCode statusCode, long count)
        {
            StatusCode = statusCode;
            Count = count;
        }
        
        public HttpStatusCode StatusCode { get; private set; }

        public long Count { get; private set; }
    }
}