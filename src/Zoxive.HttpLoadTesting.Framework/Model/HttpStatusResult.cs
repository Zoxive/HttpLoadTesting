using System.Net;
using System.Net.Http;

namespace Zoxive.HttpLoadTesting.Framework.Model
{
    public class HttpStatusResult
    {
        public HttpStatusResult(HttpResponseMessage result, long elapsedMilliseconds)
        {
            ElapsedMilliseconds = elapsedMilliseconds;
            StatusCode = result.StatusCode;
            RequestUrl = result.RequestMessage.RequestUri.ToString();
            Method = result.RequestMessage.Method.Method;
        }

        public string Method { get; private set; }

        public long ElapsedMilliseconds { get; private set; }

        public string RequestUrl { get; private set; }

        public HttpStatusCode StatusCode { get; private set; }
    }
}