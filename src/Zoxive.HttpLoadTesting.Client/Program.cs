using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zoxive.HttpLoadTesting.Client.Domain.Database;
using Zoxive.HttpLoadTesting.Client.Domain.Iteration.Repositories;
using Zoxive.HttpLoadTesting.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Core.Schedules;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
#if DEBUG
            var httpUsers = new List<IHttpUser>
            {
                new HttpUser("http://localhost")
            };
            var loadTests = new List<ILoadTest>
            {
                new LocalhostHit()
            };

            var schedule = new List<ISchedule>
            {
                new AddUsers(2, 2, 0),
                //new Duration(0.005m)
            };

            var loadTestExection = new LoadTestExecution(httpUsers, loadTests);
            Parallel.Invoke
            (
                () => Start(loadTestExection), async () =>
                {
                    // Wait for Kestrel to start...
                    // TODO callback? listen for ports?
                    await Task.Delay(1000);

                    await loadTestExection.Execute(schedule);
                }
            );
#else
            Start(null);
#endif
        }

        internal static void Start(ILoadTestExecution loadTestExecution, CancellationToken? cancellationToken = null)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .ConfigureServices(services =>
                {
                    ConfigureServices(services);

                    InitializeWithServices(loadTestExecution, services);
                })
                .Build();

            if (cancellationToken.HasValue)
            {
                host.Run(cancellationToken.Value);
            }
            else
            {
                host.Run();
            }
        }

        private static void InitializeWithServices(ILoadTestExecution loadTestExecution, IServiceCollection services)
        {
            var sp = services.BuildServiceProvider();
            var iterationResultRepository = sp.GetService<IIterationResultRepository>();
            if (loadTestExecution != null)
            {
                loadTestExecution.UserIterationFinished += LogIteration(iterationResultRepository);
            }

            DbInitializer.Initialize(sp.GetService<IDbConnection>());
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.TryAddSingleton<IDbConnection>(new SqliteConnection("Data Source=test.db"));

            services.TryAddSingleton<IIterationResultRepository>(provider => new IterationResultRepository(provider.GetService<IDbConnection>()));
        }

        private static UserIterationFinished LogIteration(IIterationResultRepository iterationResultRepository)
        {
            return iterationResult =>
            {
                Task.Run(() =>
                {
                    iterationResultRepository.Save(iterationResult);

                    DisplayCrappyResults(iterationResult);
                });
            };
        }

        private static void DisplayCrappyResults(UserIterationResult result)
        {
            var totalHttpResponseMs = 0d;

            var previous = Console.ForegroundColor;
            foreach (var statusCode in result.StatusResults)
            {
                totalHttpResponseMs += statusCode.ElapsedMilliseconds;
                if ((int)statusCode.StatusCode >= 400)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{statusCode.StatusCode} {statusCode.Method} {statusCode.RequestUrl}");
                }
            }
            Console.ForegroundColor = previous;

            var seconds = totalHttpResponseMs / 1000;

            var thinkTime = result.Elapsed.TotalSeconds - seconds;

            Console.WriteLine($"{seconds} ({thinkTime}) - {result.TestName} | {result.BaseUrl} User{result.UserNumber}.{result.Iteration}");
        }
    }

#if DEBUG
    public class LocalhostHit : ILoadTest
    {
        public string Name => "LocalhostHit";

        public Task Initialize(ILoadTestHttpClient loadTestHttpClient)
        {
            return Task.CompletedTask;
        }

        public async Task Execute(ILoadTestHttpClient loadTestHttpClient)
        {
            await loadTestHttpClient.Get("");

            //await Task.Delay(50);
        }
    }
#endif
}
