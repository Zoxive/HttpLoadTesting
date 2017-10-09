using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Zoxive.HttpLoadTesting.Client.Domain.Database;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Factories;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Repositories;
using Zoxive.HttpLoadTesting.Client.Domain.Iteration.Repositories;
using Zoxive.HttpLoadTesting.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Core.Schedules;
using Zoxive.HttpLoadTesting.Framework.Http;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
#if DEBUG
            var loadTests = new List<ILoadTest>
            {
                new LocalhostHit()
            };
            var httpUsers = new List<IHttpUser>
            {
                new HttpUser("http://localhost", loadTests)
            };

            var schedule = new List<ISchedule>
            {
                new AddUsers(2, 2, 0),
                new Duration(0.1m)
            };

            var loadTestExection = new LoadTestExecution(httpUsers);
            Parallel.Invoke
            (
                () => Start(loadTestExection, null), async () =>
                {
                    // Wait for Kestrel to start...
                    // TODO callback? listen for ports?
                    await Task.Delay(1000);

                    await loadTestExection.Execute(schedule);
                }
            );
#else
            Start(null, null);
#endif
        }

        internal static void Start(ILoadTestExecution loadTestExecution, IHttpStatusResultService httpStatusResultService, CancellationToken? cancellationToken = null)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseUrls("http://localhost:5000")
                .ConfigureServices(services =>
                {
                    ConfigureServices(services, httpStatusResultService);

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

        private static void ConfigureServices(IServiceCollection services, IHttpStatusResultService httpStatusResultService)
        {
            var now = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");

            services.AddSingleton<IDbConnection>(new SqliteConnection($"Data Source=Test_{now}.db"));

            services.AddSingleton(provider => httpStatusResultService ?? new HttpStatusResultNullService());
            services.AddSingleton<IIterationResultRepository, IterationResultRepository>();
            services.AddSingleton<IHttpStatusResultStatisticsFactory, HttpStatusResultStatisticsFactory>();
            services.AddSingleton<IHttpStatusResultRepository, HttpStatusResultRepository>();

            Domain.GraphStats.ConfigureGraphStats.ConfigureServices(services);
        }

        private static UserIterationFinished LogIteration(IIterationResultRepository iterationResultRepository)
        {
            return iterationResult =>
            {
                // in a task so it doesnt slow down the user execution
                Task.Run(() => iterationResultRepository.Save(iterationResult));
            };
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

        public async Task Execute(IUserLoadTestHttpClient loadTestHttpClient)
        {
            await loadTestHttpClient.Get("");

            //await Task.Delay(50);
        }
    }
#endif
}
