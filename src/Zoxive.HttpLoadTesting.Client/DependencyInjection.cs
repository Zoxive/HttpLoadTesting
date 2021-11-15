using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        public static IServiceCollection AddHttpLoadTesting(this IServiceCollection services, IReadOnlyList<IHttpUser> users, IReadOnlyList<ISchedule> schedules, string[] args, Action<ClientOptions>? configureOptions = null, IHttpStatusResultService? httpStatusResultService = null)
        {
            var clientOptions = ClientOptions.FromArgs(args);

            configureOptions?.Invoke(clientOptions);

            return AddHttpLoadTesting(services, users, schedules, clientOptions, httpStatusResultService);
        }

        public static IServiceCollection AddHttpLoadTesting(this IServiceCollection services, IReadOnlyList<IHttpUser> users, IReadOnlyList<ISchedule> schedules, ClientOptions options, IHttpStatusResultService? httpStatusResultService = null)
        {
            var testingParams = new HttpLoadTestingParams(users, schedules, options, httpStatusResultService);

            var hostlifetime = services.FirstOrDefault(x => x.ServiceType == typeof(IHostLifetime));
            if (hostlifetime != null)
                services.Remove(hostlifetime);

            services.AddSingleton<IHostLifetime, ConsoleTestHostLifetime>();

            services.AddSingleton<TestExecutionToken>();

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

            services.AddSingleton<UserExecutingQueue>();
            services.AddHostedService<UserExecutionBackgroundService>();

            Client.Domain.GraphStats.ConfigureGraphStats.ConfigureServices(services);
        }
    }

    public sealed class ConsoleTestHostLifetime : IHostLifetime, IDisposable
    {
        private readonly ConsoleLifetimeOptions Options;
        private readonly IHostEnvironment Environment;
        private readonly IHostApplicationLifetime ApplicationLifetime;
        private readonly TestExecutionToken _testExecutionToken;
        private readonly HostOptions HostOptions;
        private readonly ILogger Logger;

        public ConsoleTestHostLifetime(IOptions<ConsoleLifetimeOptions> options, IHostEnvironment environment, IHostApplicationLifetime applicationLifetime, IOptions<HostOptions> hostOptions, ILoggerFactory loggerFactory, TestExecutionToken testExecutionToken)
        {
            Options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            ApplicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
            _testExecutionToken = testExecutionToken;
            HostOptions = hostOptions?.Value ?? throw new ArgumentNullException(nameof(hostOptions));
            Logger = loggerFactory.CreateLogger("Microsoft.Hosting.Lifetime");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private CancellationTokenRegistration? _applicationStartedRegistration;
        private CancellationTokenRegistration? _applicationStoppingRegistration;
        public Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            if (!Options.SuppressStatusMessages)
            {
                _applicationStartedRegistration = ApplicationLifetime.ApplicationStarted.Register(state =>
                    {
                        ((ConsoleTestHostLifetime)state!).OnApplicationStarted();
                    },
                    this);
                _applicationStoppingRegistration = ApplicationLifetime.ApplicationStopping.Register(state =>
                    {
                        ((ConsoleTestHostLifetime)state!).OnApplicationStopping();
                    },
                    this);
            }

            RegisterShutdownHandlers();

            // Console applications start immediately.
            return Task.CompletedTask;
        }

        private void OnApplicationStarted()
        {
            Logger.LogInformation("Application started. Press Ctrl+C to stop running tests.");
            Logger.LogInformation("Press Ctrl+C again to shutdown.");
            Logger.LogInformation("Hosting environment: {envName}", Environment.EnvironmentName);
            Logger.LogInformation("Content root path: {contentRoot}", Environment.ContentRootPath);
        }

        private void OnApplicationStopping()
        {
            Logger.LogInformation("Application is shutting down...");
        }

        public void Dispose()
        {
            _sigIntRegistration?.Dispose();
            _sigQuitRegistration?.Dispose();
            _sigTermRegistration?.Dispose();
        }

        private PosixSignalRegistration? _sigIntRegistration;
        private PosixSignalRegistration? _sigQuitRegistration;
        private PosixSignalRegistration? _sigTermRegistration;

        private void RegisterShutdownHandlers()
        {
            Action<PosixSignalContext> handler = HandlePosixSignal;
            _sigIntRegistration?.Dispose();
            _sigIntRegistration = PosixSignalRegistration.Create(PosixSignal.SIGINT, handler);
            _sigQuitRegistration?.Dispose();
            _sigQuitRegistration = PosixSignalRegistration.Create(PosixSignal.SIGQUIT, handler);
            _sigTermRegistration?.Dispose();
            _sigTermRegistration = PosixSignalRegistration.Create(PosixSignal.SIGTERM, handler);
        }

        private bool _firstStopCalled = false;
        private void HandlePosixSignal(PosixSignalContext context)
        {
            if (!_firstStopCalled)
            {
                _firstStopCalled = true;
                Logger.LogInformation("Stopping Test Execution..");

                _testExecutionToken.Stop();

                // cancel token that stops tests..
                Logger.LogInformation("Press Ctrl+C again to shutdown.");
                return;
            }

            context.Cancel = true;
            ApplicationLifetime.StopApplication();
        }
    }

    public sealed class TestExecutionToken : IDisposable
    {
        private readonly CancellationTokenSource _cancelSource;

        public TestExecutionToken()
        {
            _cancelSource = new CancellationTokenSource();

        }

        public void Stop()
        {
            _cancelSource.Cancel();
        }

        public CancellationToken Token => _cancelSource.Token;

        public void Dispose()
        {
            _cancelSource.Dispose();
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