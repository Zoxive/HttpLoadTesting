using System.Net;
using System.Net.Http;

namespace Zoxive.HttpLoadTesting.Framework.Model
{
    public class HttpStatusResult
    {
        public HttpStatusResult(HttpResponseMessage result, double elapsedMilliseconds, double requestStartMilliseconds)
        {
            ElapsedMilliseconds = elapsedMilliseconds;
            StatusCode = result.StatusCode;
            RequestUrl = result.RequestMessage?.RequestUri?.ToString();
            Method = result.RequestMessage?.Method.Method;
            RequestStartedMs = requestStartMilliseconds;
        }

        public HttpStatusResult(long id, string? method, double elapsedMilliseconds, string? requestUrl, HttpStatusCode statusCode, double requestStartMilliseconds)
        {
            Id = id;
            Method = method;
            ElapsedMilliseconds = elapsedMilliseconds;
            RequestUrl = requestUrl;
            StatusCode = statusCode;
            RequestStartedMs = requestStartMilliseconds;
        }

        public long Id { get; }

        public string? Method { get; }

        public double ElapsedMilliseconds { get; }

        public string? RequestUrl { get; }

        public HttpStatusCode StatusCode { get; }

        public double RequestStartedMs { get; }
    }
}