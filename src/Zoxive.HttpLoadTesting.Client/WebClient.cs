using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Zoxive.HttpLoadTesting.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client
{
    public class WebClient
    {
        public static void Run(ILoadTestExecution testExecution, IReadOnlyList<ISchedule> schedule, IHttpStatusResultService httpStatusResultService, ClientOptions clientOptions)
        {
            var cancellationSource = new CancellationTokenSource();
            Console.CancelKeyPress += ((sender, cancelEventArgs) =>
            {
                cancellationSource.Cancel();
                cancelEventArgs.Cancel = true;
            });

            Task RunTests()
            {
                return testExecution.Execute(schedule, cancellationSource.Token);
            }

            var executeTests = TestStartup(RunTests, clientOptions);
            var webUi = Program.StartAsync(testExecution, httpStatusResultService, cancellationSource.Token, clientOptions);

            Task.WaitAll(executeTests, webUi);
        }

        public static void Run(ILoadTestExecution testExecution, IReadOnlyList<ISchedule> schedule, IHttpStatusResultService httpStatusResultService, string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            var clientOptions = new ClientOptions(config.GetValue<string>("databaseFile"));

            Run(testExecution, schedule, httpStatusResultService, clientOptions);
        }

        private static Task TestStartup(Func<Task> runTests, ClientOptions clientOptions)
        {
            // TODO allow user to say if viewing
            if (clientOptions.Viewing)
            {
                Console.WriteLine("Viewing Existing Data..");
                return Task.CompletedTask;
            }

            Console.WriteLine("Running Tests..");
            return runTests();
        }
    }
}