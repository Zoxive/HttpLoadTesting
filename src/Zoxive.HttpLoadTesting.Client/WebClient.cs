using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Framework.Core;

namespace Zoxive.HttpLoadTesting.Client
{
    public class WebClient
    {
        public static void Run(ILoadTestExecution testExecution, IReadOnlyList<ISchedule> schedule)
        {
            var cancellationSource = new CancellationTokenSource();

            Console.CancelKeyPress += ((sender, args) =>
            {
                cancellationSource.Cancel();
                args.Cancel = true;
            });

            var options = new ParallelOptions
            {
                CancellationToken = cancellationSource.Token 
            };

            Parallel.Invoke
            (
                options,
                () => testExecution.Execute(schedule, cancellationSource.Token),
                () => Program.Start(testExecution, cancellationSource.Token)
            );
        }
    }
}