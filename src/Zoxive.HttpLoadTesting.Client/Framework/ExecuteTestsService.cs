using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Zoxive.HttpLoadTesting.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Framework
{
    public class ExecuteTestsService : BackgroundService
    {
        private readonly ClientOptions _clientOptions;
        private readonly ILoadTestExecution _loadTestExecution;
        private readonly IReadOnlyList<ISchedule> _schedules;
        private readonly ISaveIterationQueue _saveIterationQueue;

        public ExecuteTestsService
        (
            ClientOptions clientOptions,
            ILoadTestExecution loadTestExecution,
            IReadOnlyList<ISchedule> schedules,
            ISaveIterationQueue saveIterationQueue
        )
        {
            _clientOptions = clientOptions;
            _loadTestExecution = loadTestExecution;
            _schedules = schedules;
            _saveIterationQueue = saveIterationQueue;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"Using DatabaseFile: {_clientOptions.DatabaseFile}");

            // TODO allow user to say if viewing
            if (_clientOptions.Viewing)
            {
                Console.WriteLine("Viewing Existing Data..");
                return Task.CompletedTask;
            }

            Console.WriteLine("Running Tests..");

            _loadTestExecution.UserIterationFinished += LogIteration(_saveIterationQueue);

            return _loadTestExecution.Execute(_schedules, stoppingToken);
        }

        private static UserIterationFinished LogIteration(ISaveIterationQueue iterationQueueRepository)
        {
            return iterationQueueRepository.Queue;
        }
    }
}
