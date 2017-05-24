using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Zoxive.HttpLoadTesting.Client;
using Zoxive.HttpLoadTesting.Examples.Examples.Tests;
using Zoxive.HttpLoadTesting.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Core.Schedules;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Examples.Examples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var schedule = new List<ISchedule>
            {
                new AddUsers(totalUsers: 2, usersEvery: 2, seconds: 5),
                new Duration(0.25m)
            };

            var tests = new List<ILoadTest>
            {
                new ReadAPostCreateAComment()
            };

            var users = new List<IHttpUser>
            {
                new HttpUser("https://jsonplaceholder.typicode.com/")
                {
                    AlterHttpClient = SetHttpClientProperties,
                    AlterHttpClientHandler = SetHttpClientHandlerProperties,
                    AlterHttpRequestMessage = SetHttpRequestHeaders
                },
                new HttpUser("https://jsonplaceholder.typicode.com/")
                {
                    AlterHttpClient = SetHttpClientProperties,
                    AlterHttpClientHandler = SetHttpClientHandlerProperties,
                    AlterHttpRequestMessage = SetHttpRequestHeadersUser2
                }
            };

            var testExecution = new LoadTestExecution(users, tests);

            Zoxive.HttpLoadTesting.Client.WebClient.Run(testExecution, schedule, null);
        }

        private static void SetHttpClientProperties(HttpClient httpClient)
        {
            httpClient.Timeout = new TimeSpan(0, 1, 0);

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.ExpectContinue = false;
            httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
        }

        public static void SetHttpClientHandlerProperties(HttpClientHandler httpClientHandler)
        {
            httpClientHandler.AllowAutoRedirect = true;
            httpClientHandler.AutomaticDecompression = DecompressionMethods.Deflate;
        }

        public static void SetHttpRequestHeaders(HttpRequestMessage request)
        {
            request.Headers.Add("Awesome-Custom-Header", "request modified");
        }

        public static void SetHttpRequestHeadersUser2(HttpRequestMessage request)
        {
            request.Headers.Add("Awesome-Custom-Header", "request modified for user 2");
        }
    }
}
