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

            var t = new Timer(_ => { cancellationSource.Cancel(); }, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(0));

            var webUi = Program.StartAsync(testExecution, schedule, httpStatusResultService, cancellationSource.Token, clientOptions);

            Task.WaitAll(webUi);
        }

        public static void Run(ILoadTestExecution testExecution, IReadOnlyList<ISchedule> schedule, IHttpStatusResultService httpStatusResultService, string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            var clientOptions = new ClientOptions(config.GetValue<string>("databaseFile"));

            Run(testExecution, schedule, httpStatusResultService, clientOptions);
        }
    }
}