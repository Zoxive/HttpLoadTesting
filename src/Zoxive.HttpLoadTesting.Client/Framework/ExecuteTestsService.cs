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
        private readonly TestExecutionToken _testExecutionToken;

        public ExecuteTestsService
        (
            ClientOptions clientOptions,
            ILoadTestExecution loadTestExecution,
            IReadOnlyList<ISchedule> schedules,
            ISaveIterationQueue saveIterationQueue,
            TestExecutionToken testExecutionToken
        )
        {
            _clientOptions = clientOptions;
            _loadTestExecution = loadTestExecution;
            _schedules = schedules;
            _saveIterationQueue = saveIterationQueue;
            _testExecutionToken = testExecutionToken;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"Using DatabaseFile: {_clientOptions.DatabaseFile}");

            // TODO allow user to say if viewing
            if (_clientOptions.Viewing)
            {
                Console.WriteLine("Viewing Existing Data..");
                return;
            }

            Console.WriteLine("Running Tests..");

            _loadTestExecution.UserIterationFinished += LogIteration(_saveIterationQueue);

            var token = _testExecutionToken.Token;

            try
            {
                await _loadTestExecution.Execute(_schedules, token);
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Test Scheduler Stopped.");
                // eat the token cancelled exception
            }
        }

        private static UserIterationFinished LogIteration(ISaveIterationQueue iterationQueueRepository)
        {
            return iterationQueueRepository.Queue;
        }
    }
}
