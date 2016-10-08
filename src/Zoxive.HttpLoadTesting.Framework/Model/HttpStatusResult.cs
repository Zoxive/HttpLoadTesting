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

        public HttpStatusResult(string method, long elapsedMilliseconds, string requestUrl, HttpStatusCode statusCode)
        {
            Method = method;
            ElapsedMilliseconds = elapsedMilliseconds;
            RequestUrl = requestUrl;
            StatusCode = statusCode;
        }

        public string Method { get; private set; }

        public long ElapsedMilliseconds { get; private set; }

        public string RequestUrl { get; private set; }

        public HttpStatusCode StatusCode { get; private set; }
    }
}