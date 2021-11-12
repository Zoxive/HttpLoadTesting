using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Examples.Tests;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Zoxive.HttpLoadTesting;
using Zoxive.HttpLoadTesting.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Core.Schedules;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Examples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var schedule = new List<ISchedule>
            {
                /*
                new AddUsers(totalUsers: 50, usersEvery: 10, seconds: 1),
                new Duration(0.2m),
                new AddUsers(totalUsers: 50, usersEvery: 10, seconds: 1),
                new Duration(0.2m),
                new AddUsers(totalUsers: 50, usersEvery: 10, seconds: 1),
                new Duration(0.2m),
                new AddUsers(totalUsers: 50, usersEvery: 10, seconds: 1),
                new Duration(2m),
                */
                /*
                new AddUsers(totalUsers: 30, usersEvery: 10, seconds: 1),
                new Duration(2m),
                */
                new AddUsers(totalUsers: 1, usersEvery: 1, seconds: 1),
            };

            var tests = new List<ILoadTest>
            {
                //new ReadAPostCreateAComment()
                new LocalhostHit()
            };

            var users = new List<IHttpUser>
            {
                new HttpUser("http://localhost", tests)
                {
                    AlterHttpClient = SetHttpClientProperties,
                    CreateHttpMessageHandler = SetHttpClientHandlerProperties,
                    AlterHttpRequestMessage = SetHttpRequestHeaders
                },
                new HttpUser("http://localhost", tests)
                {
                    AlterHttpClient = SetHttpClientProperties,
                    CreateHttpMessageHandler = SetHttpClientHandlerProperties,
                    AlterHttpRequestMessage = SetHttpRequestHeadersUser2
                }
            };


            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((services) =>
                {
                    services.AddHttpLoadTesting(users, schedule, args);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

            host.Build().Run();
        }

        private static void SetHttpClientProperties(HttpClient httpClient)
        {
            httpClient.Timeout = new TimeSpan(0, 1, 0);

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.ExpectContinue = false;
            httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
        }

        private static HttpMessageHandler SetHttpClientHandlerProperties()
        {
            return new HttpClientHandler
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.Deflate
            };
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
