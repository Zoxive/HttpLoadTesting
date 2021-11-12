using System.Collections.Generic;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Zoxive.HttpLoadTesting.Client;
using Zoxive.HttpLoadTesting.Client.Domain.Database;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Factories;
using Zoxive.HttpLoadTesting.Client.Domain.HttpStatusResult.Repositories;
using Zoxive.HttpLoadTesting.Client.Domain.Iteration.Repositories;
using Zoxive.HttpLoadTesting.Client.Framework;
using Zoxive.HttpLoadTesting.Client.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddHttpLoadTesting(this IServiceCollection services, IReadOnlyList<IHttpUser> users, IReadOnlyList<ISchedule> schedules, string[] args, IHttpStatusResultService? httpStatusResultService = null)
        {
            var clientOptions = ClientOptions.FromArgs(args);
            return AddHttpLoadTesting(services, users, schedules, clientOptions, httpStatusResultService);
        }

        public static IServiceCollection AddHttpLoadTesting(this IServiceCollection services, IReadOnlyList<IHttpUser> users, IReadOnlyList<ISchedule> schedules, ClientOptions options, IHttpStatusResultService? httpStatusResultService = null)
        {
            var testingParams = new HttpLoadTestingParams(users, schedules, options, httpStatusResultService);

            ConfigureServices(services, httpStatusResultService, options.DatabaseFile);
            services.AddSingleton(testingParams);
            services.AddSingleton(ioc => ioc.GetRequiredService<LoadTestExecutionFactory>().Create(users));
            services.AddSingleton(schedules);
            services.AddSingleton(options);

            return services;
        }

        private static void ConfigureServices(IServiceCollection services, IHttpStatusResultService? httpStatusResultService, string databaseFile)
        {
            var connectionString = $"Data Source={databaseFile};cache=shared";

            var readerConnection = new SqliteConnection(connectionString);

            services.AddSingleton<HostRef>();
            services.AddSingleton<LoadTestExecutionFactory>();
            services.AddSingleton<IDbReader>(new Db(readerConnection));

            services.AddSingleton(provider => httpStatusResultService ?? new HttpStatusResultNullService());

#pragma warning disable CA2000
            services.AddSingleton<SqliteConnection>(_ =>
            {
                var connection = new SqliteConnection($"Data Source={databaseFile};cache=shared");
                connection.Execute("PRAGMA journal_mode = MEMORY;");
                return connection;
            });
#pragma warning restore CA2000

            services.AddSingleton<IDbWriter>(provider =>
            {
                var connection = provider.GetRequiredService<SqliteConnection>();
                var db = new Db(connection);
                DbInitializer.Initialize(db);
                return db;
            });
            services.AddSingleton<IIterationResultRepository>(i => new IterationResultRepository(i.GetRequiredService<IDbWriter>()));

            services.AddSingleton<IHttpStatusResultStatisticsFactory, HttpStatusResultStatisticsFactory>();
            services.AddSingleton<IResultRepository, ResultRepository>();
            services.AddSingleton<IRequestResultRepository, RequestResultRepository>();
            services.AddSingleton<ITestResultRepository, TestResultRepository>();

            services.AddHostedService<ExecuteTestsService>();

            services.AddSingleton<ISimpleTransaction, SimpleTransaction>();

            services.AddSingleton<ISaveIterationQueue, SaveIterationQueueQueue>();
            services.AddHostedService(ioc =>
            {
                var transaction = ioc.GetRequiredService<ISimpleTransaction>();
                var repo = ioc.GetRequiredService<IIterationResultRepository>();
                var queue = ioc.GetRequiredService<ISaveIterationQueue>();
                return new SaveIterationResultBackgroundService(transaction, repo, queue, "File");
            });

            Client.Domain.GraphStats.ConfigureGraphStats.ConfigureServices(services);
        }
    }

    public sealed class HttpLoadTestingParams
    {
        public IReadOnlyList<IHttpUser> Users { get; }
        public IReadOnlyList<ISchedule> Schedules { get; }
        public ClientOptions Options { get; }
        public IHttpStatusResultService? HttpStatusResultService { get; }

        public HttpLoadTestingParams(IReadOnlyList<IHttpUser> users, IReadOnlyList<ISchedule> schedules, ClientOptions options, IHttpStatusResultService? httpStatusResultService)
        {
            Users = users;
            Schedules = schedules;
            Options = options;
            HttpStatusResultService = httpStatusResultService;
        }
    }
}