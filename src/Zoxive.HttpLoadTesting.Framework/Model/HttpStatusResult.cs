using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;

namespace Zoxive.HttpLoadTesting.Framework.Model
{
    public class HttpStatusResult
    {
        public HttpStatusResult(HttpResponseMessage result, double elapsedMilliseconds, long requestStartTick)
        {
            ElapsedMilliseconds = elapsedMilliseconds;
            StatusCode = result.StatusCode;
            RequestUrl = result.RequestMessage.RequestUri.ToString();
            Method = result.RequestMessage.Method.Method;
            RequestStartTick = requestStartTick;
        }

        public HttpStatusResult(long id, string method, double elapsedMilliseconds, string requestUrl, HttpStatusCode statusCode, long requestStartTick)
        {
            Id = id;
            Method = method;
            ElapsedMilliseconds = elapsedMilliseconds;
            RequestUrl = requestUrl;
            StatusCode = statusCode;
            RequestStartTick = requestStartTick;
        }

        public long Id { get; }

        public string Method { get; }

        public double ElapsedMilliseconds { get; }

        public string RequestUrl { get; }

        public HttpStatusCode StatusCode { get; }

        public long RequestStartTick { get; }
    }
}