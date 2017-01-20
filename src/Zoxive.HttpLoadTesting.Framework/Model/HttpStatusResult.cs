﻿using System;
using System.Net;
using System.Net.Http;

namespace Zoxive.HttpLoadTesting.Framework.Model
{
    public class HttpStatusResult
    {
        public HttpStatusResult(HttpResponseMessage result, long elapsedTicks, long requestStartTick)
        {
            ElapsedMilliseconds = (double)elapsedTicks / TimeSpan.TicksPerMillisecond;
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

        public long Id { get; private set; }

        public string Method { get; private set; }

        public double ElapsedMilliseconds { get; private set; }

        public string RequestUrl { get; private set; }

        public HttpStatusCode StatusCode { get; private set; }

        public long RequestStartTick { get; }
    }
}