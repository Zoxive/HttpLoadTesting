using System;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
        internal static Task StartAsync(ILoadTestExecution loadTestExecution, IHttpStatusResultService httpStatusResultService, CancellationToken cancellationToken, ClientOptions options)
        {
            Console.WriteLine($"Using DatbaseFile: {options.DatabaseFile}");

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseUrls("http://localhost:5000")
                .ConfigureServices(services =>
                {
                    ConfigureServices(services, httpStatusResultService, options.DatabaseFile);

                    InitializeWithServices(loadTestExecution, services);
                })
                .Build();

            return host.RunAsync(cancellationToken);
        }

        private static void InitializeWithServices(ILoadTestExecution loadTestExecution, IServiceCollection services)
        {
            var sp = services.BuildServiceProvider();
            var saveIterationResult = sp.GetRequiredService<ISaveIterationResult>();
            if (loadTestExecution != null)
            {
                loadTestExecution.UserIterationFinished += LogIteration(saveIterationResult);
            }

            DbInitializer.Initialize(sp.GetService<IDbWriter>());
        }

        private static void ConfigureServices(IServiceCollection services, IHttpStatusResultService httpStatusResultService, string databaseFile)
        {
            /*
            var writerConnection = new SqliteConnection($"Data Source={databaseFile};cache=shared");
            var readerConnection = new SqliteConnection($"Data Source={databaseFile};cache=shared");
            */
            var writerConnection = new SqliteConnection($"Data Source={databaseFile};mode=memory;cache=shared");
            var readerConnection = new SqliteConnection($"Data Source={databaseFile};mode=memory;cache=shared");

            var dbWriter = new Db(writerConnection);

            services.AddSingleton<IDbWriter>(dbWriter);
            services.AddSingleton<IDbReader>(new Db(readerConnection));

            var iterationResultRepository = new IterationResultRepository(dbWriter);

            services.AddSingleton(provider => httpStatusResultService ?? new HttpStatusResultNullService());
            services.AddSingleton<IIterationResultRepository>(iterationResultRepository);
            services.AddSingleton<IHttpStatusResultStatisticsFactory, HttpStatusResultStatisticsFactory>();
            services.AddSingleton<IHttpStatusResultRepository, HttpStatusResultRepository>();

            var backgroundService = new SaveIterationResultBackgroundService(iterationResultRepository);

            services.AddSingleton<IHostedService>(backgroundService);
            services.AddSingleton<ISaveIterationResult>(backgroundService);

            Domain.GraphStats.ConfigureGraphStats.ConfigureServices(services);
        }

        private static UserIterationFinished LogIteration(ISaveIterationResult iterationResultRepository)
        {
            return iterationResultRepository.Queue;
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
