using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Zoxive.HttpLoadTesting.Client.Domain.Database;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Factories;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Repositories;
using Zoxive.HttpLoadTesting.Client.Domain.Iteration.Repositories;
using Zoxive.HttpLoadTesting.Client.Framework;
using Zoxive.HttpLoadTesting.Client.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Console.WriteLine("No op");
            return 0;
        }

        internal static Task StartAsync
        (
            IReadOnlyList<IHttpUser> users,
            IReadOnlyList<ISchedule> schedules,
            IHttpStatusResultService httpStatusResultService,
            ClientOptions options
        )
        {
            var host = WebHost.CreateDefaultBuilder<Startup>(new string[0])
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUrls("http://localhost:5000")
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(services, httpStatusResultService, options.DatabaseFile);
                    services.AddSingleton<LoadTestExecutionFactory>();
                    services.AddSingleton(ioc => ioc.GetRequiredService<LoadTestExecutionFactory>().Create(users));
                    services.AddSingleton(schedules);
                    services.AddSingleton(options);
                })
                .Build();

            var stopWebUi = new CancellationTokenSource();

            if (!options.StopApplicationWhenComplete)
            {
                options.CancelTokenSource.Token.Register(Cancel(stopWebUi));
            }
            else
            {
                options.CancelTokenSource.Token.Register(() =>
                {
                    stopWebUi.Cancel();
                });
            }

            return host.RunAsync(stopWebUi.Token);
        }

        private static Action Cancel(CancellationTokenSource cancellationSource)
        {
            return () =>
            {
                Console.CancelKeyPress += ((sender, cancelEventArgs) =>
                {
                    cancellationSource.Cancel();
                    cancelEventArgs.Cancel = true;
                });

                var previous = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("CANCELING");
                Console.WriteLine("Press ctrl+c again to stop the webui");

                Console.ForegroundColor = previous;
            };
        }

        private static void ConfigureServices(IServiceCollection services, IHttpStatusResultService httpStatusResultService, string databaseFile)
        {
            var connectionString = $"Data Source={databaseFile};cache=shared";

            var readerConnection = new SqliteConnection(connectionString);

            services.AddSingleton<IDbReader>(new Db(readerConnection));

            services.AddSingleton(provider => httpStatusResultService ?? new HttpStatusResultNullService());
            services.AddSingleton<IIterationResultRepository>(CreateIterationResultRepository(databaseFile, out var dbWriter));

            services.AddSingleton<IDbWriter>(dbWriter);

            services.AddSingleton<IHttpStatusResultStatisticsFactory, HttpStatusResultStatisticsFactory>();
            services.AddSingleton<IResultRepository, ResultRepository>();
            services.AddSingleton<IRequestResultRepository, RequestResultRepository>();
            services.AddSingleton<ITestResultRepository, TestResultRepository>();

            services.AddSingleton<IHostedService, ExecuteTestsService>();

            services.AddSingleton<ISimpleTransaction, SimpleTransaction>();

            services.AddSingleton<ISaveIterationQueue, SaveIterationQueueQueue>();
            services.AddSingleton<IHostedService, SaveIterationResultBackgroundService>(ioc =>
            {
                var transaction = ioc.GetRequiredService<ISimpleTransaction>();
                var repo = ioc.GetRequiredService<IIterationResultRepository>();
                var queue = ioc.GetRequiredService<ISaveIterationQueue>();
                return new SaveIterationResultBackgroundService(transaction, repo, queue, "File");
            });

            Domain.GraphStats.ConfigureGraphStats.ConfigureServices(services);
        }

        public static IterationResultRepository CreateIterationResultRepository(string databaseFile, out IDbWriter fileDb)
        {
            var connection = new SqliteConnection($"Data Source={databaseFile};cache=shared");
            fileDb = new Db(connection);

            var fileResultRepository = new IterationResultRepository(fileDb);
            DbInitializer.Initialize(fileDb);

            // Looks like we dont really need this
            //connection.Execute("PRAGMA synchronous = OFF;");
            connection.Execute("PRAGMA journal_mode = MEMORY;");

            return fileResultRepository;
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