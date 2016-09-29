using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Client.Repositories;
using Zoxive.HttpLoadTesting.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client
{
    public class WebClient
    {
        public static void Run(ILoadTestExecution testExecution, IReadOnlyList<ISchedule> schedule, string contentRoot)
        {
            var stepResultsRepository = new IterationResultRepository();

            testExecution.UserIterationFinished += LogResults(stepResultsRepository);

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
                () => Program.Start(stepResultsRepository, contentRoot, cancellationSource.Token)
            );
        }

        private static UserIterationFinished LogResults(IIterationResultRepository iterationResultsRepository)
        {
            return (result) =>
            {
                iterationResultsRepository.Save(result);

                Task.Run(() => DisplayCrappyResults(result));
            };
        }

        private static void DisplayCrappyResults(UserIterationResult result)
        {
            var totalHttpResponseMs = 0d;

            var previous = Console.ForegroundColor;
            foreach (var statusCode in result.StatusResults)
            {
                totalHttpResponseMs += statusCode.ElapsedMilliseconds;
                if ((int)statusCode.StatusCode >= 400)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{statusCode.StatusCode} {statusCode.Method} {statusCode.RequestUrl}");
                }
            }
            Console.ForegroundColor = previous;

            var seconds = totalHttpResponseMs / 1000;

            var thinkTime = result.Elapsed.TotalSeconds - seconds;

            Console.WriteLine($"{seconds} ({thinkTime}) - {result.TestName} | {result.BaseUrl} User{result.UserNumber}.{result.Iteration}");
        }
    }
}