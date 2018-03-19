using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Zoxive.HttpLoadTesting.Client.Domain.Database;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Factories;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Repositories;
using Zoxive.HttpLoadTesting.Client.Domain.Iteration.Repositories;
using Zoxive.HttpLoadTesting.Client.Framework;
using Zoxive.HttpLoadTesting.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client
{
    public class Program
    {
        internal static Task StartAsync
        (
            ILoadTestExecution loadTestExecution,
            IReadOnlyList<ISchedule> schedules,
            IHttpStatusResultService httpStatusResultService,
            CancellationToken cancellationToken,
            ClientOptions options
        )
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseUrls("http://localhost:5000")
                .ConfigureServices(services =>
                {
                    ConfigureServices(services, httpStatusResultService, options.DatabaseFile);

                    services.AddSingleton(loadTestExecution);
                    services.AddSingleton(schedules);
                    services.AddSingleton(options);

                    InitializeWithServices(loadTestExecution, services);
                })
                .Build();

            return host.RunAsync(cancellationToken);
        }

        private static void InitializeWithServices(ILoadTestExecution loadTestExecution, IServiceCollection services)
        {
            var sp = services.BuildServiceProvider();
            var saveIterationResult = sp.GetRequiredService<IEnumerable<ISaveIterationResult>>();
            if (loadTestExecution != null)
            {
                loadTestExecution.UserIterationFinished += LogIteration(saveIterationResult);
            }
        }

        private static void ConfigureServices(IServiceCollection services, IHttpStatusResultService httpStatusResultService, string databaseFile)
        {
            var connectionString = $"Data Source={databaseFile};cache=shared";

            var readerConnection = new SqliteConnection(connectionString);

            services.AddSingleton<IDbReader>(new Db(readerConnection));

            var iterationResultRepository = CreateFileRepository(databaseFile);

            services.AddSingleton(provider => httpStatusResultService ?? new HttpStatusResultNullService());
            services.AddSingleton<IIterationResultRepository>(iterationResultRepository);
            services.AddSingleton<IHttpStatusResultStatisticsFactory, HttpStatusResultStatisticsFactory>();
            services.AddSingleton<IHttpStatusResultRepository, HttpStatusResultRepository>();

            services.AddSingleton<IHostedService, ExecuteTestsService>();

            var inMemorySave = new SaveIterationResultBackgroundService(iterationResultRepository, "File");
            services.AddSingleton<ISaveIterationResult>(inMemorySave);
            services.AddSingleton<IHostedService>(inMemorySave);

            Domain.GraphStats.ConfigureGraphStats.ConfigureServices(services);
        }

        public static IterationResultRepository CreateFileRepository(string databaseFile)
        {
            var connection = new SqliteConnection($"Data Source={databaseFile};cache=shared");
            var fileDb = new Db(connection);

            var fileResultRepository = new IterationResultRepository(fileDb);
            DbInitializer.Initialize(fileDb);

            connection.Execute("PRAGMA synchronous = OFF;");
            connection.Execute("PRAGMA journal_mode = MEMORY;");

            return fileResultRepository;
        }

        private static UserIterationFinished LogIteration(IEnumerable<ISaveIterationResult> iterationResultRepository)
        {
            return result =>
            {
                foreach (var repository in iterationResultRepository)
                {
                    repository.Queue(result);
                }
            };
        }
    }

    public class Db : IDbWriter, IDbReader
    {
        public Db(IDbConnection connection)
        {
            Connection = connection;
        }

        public IDbConnection Connection { get; }
    }

    public interface IDbWriter
    {
        IDbConnection Connection { get; }
    }

    public interface IDbReader
    {
        IDbConnection Connection { get; }
    }
}
