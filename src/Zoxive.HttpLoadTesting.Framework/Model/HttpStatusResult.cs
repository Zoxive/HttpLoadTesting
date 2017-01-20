﻿using System;
using System.Net;
using System.Net.Http;

namespace Zoxive.HttpLoadTesting.Framework.Model
{
    public class HttpStatusResult
    {
        public HttpStatusResult(HttpResponseMessage result, long elapsedTicks)
        {
            ElapsedMilliseconds = (double)elapsedTicks / TimeSpan.TicksPerMillisecond;
            StatusCode = result.StatusCode;
            RequestUrl = result.RequestMessage.RequestUri.ToString();
            Method = result.RequestMessage.Method.Method;
        }

        public HttpStatusResult(long id, string method, double elapsedMilliseconds, string requestUrl, HttpStatusCode statusCode)
        {
            Id = id;
            Method = method;
            ElapsedMilliseconds = elapsedMilliseconds;
            RequestUrl = requestUrl;
            StatusCode = statusCode;
        }

        public long Id { get; private set; }

        public string Method { get; private set; }

        public double ElapsedMilliseconds { get; private set; }

        public string RequestUrl { get; private set; }

        public HttpStatusCode StatusCode { get; private set; }
    }
}