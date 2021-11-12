using System;
using System.Collections.Generic;
using Zoxive.HttpLoadTesting.Client.Pages;

namespace Zoxive.HttpLoadTesting.Client.Framework.Model
{
    public class HttpStatusResultDistincts
    {
        public HttpStatusResultDistincts(IEnumerable<string> tests, IEnumerable<string> methodsResult, IEnumerable<string> requestUrlUrlsResult, IEnumerable<int> statusCodes)
        {
            Tests = tests;
            Methods = methodsResult;
            RequestUrls = requestUrlUrlsResult;
            StatusCodes = statusCodes;
        }

        public IEnumerable<CollationType> CollationTypes { get; } = new[] { CollationType.Requests, CollationType.Tests };

        public IEnumerable<string> Tests { get; }

        public IEnumerable<string> RequestUrls { get; }

        public IEnumerable<string> Methods { get; }

        public IEnumerable<int> StatusCodes { get; }

        public static HttpStatusResultDistincts Empty = new(ArraySegment<string>.Empty, ArraySegment<string>.Empty, ArraySegment<string>.Empty, ArraySegment<int>.Empty);
    }
}