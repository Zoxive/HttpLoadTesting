using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Framework.Core;

namespace Zoxive.HttpLoadTesting.Client
{
    public class WebClient
    {
        public static void Run(ILoadTestExecution testExecution, IReadOnlyList<ISchedule> schedule, IHttpStatusResultService httpStatusResultService)
        {
            var cancellationSource = new CancellationTokenSource();

            Console.CancelKeyPress += ((sender, args) =>
            {
                cancellationSource.Cancel();
                args.Cancel = true;
            });

            var executeTests = testExecution.Execute(schedule, cancellationSource.Token);
            var webUi = Program.StartAsync(testExecution, httpStatusResultService, cancellationSource.Token);

            Task.WaitAll(executeTests, webUi);
        }
    }
}