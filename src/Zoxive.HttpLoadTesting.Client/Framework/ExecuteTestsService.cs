using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Framework
{
    public class ExecuteTestsService : BackgroundService
    {
        private readonly ClientOptions _clientOptions;
        private readonly ILoadTestExecution _loadTestExecution;
        private readonly IReadOnlyList<ISchedule> _schedules;
        private readonly ISaveIterationResult _saveIterationResult;

        public ExecuteTestsService(ClientOptions clientOptions, ILoadTestExecution loadTestExecution, IReadOnlyList<ISchedule> schedules, ISaveIterationResult saveIterationResult)
        {
            _clientOptions = clientOptions;
            _loadTestExecution = loadTestExecution;
            _schedules = schedules;
            _saveIterationResult = saveIterationResult;
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

            _loadTestExecution.UserIterationFinished += LogIteration(_saveIterationResult);

            return _loadTestExecution.Execute(_schedules, stoppingToken);
        }

        private static UserIterationFinished LogIteration(ISaveIterationResult iterationResultRepository)
        {
            return iterationResultRepository.Queue;
        }
    }
}
