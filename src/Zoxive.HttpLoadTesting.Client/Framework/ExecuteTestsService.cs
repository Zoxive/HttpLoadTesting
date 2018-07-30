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
        private readonly ISaveIterationResult _saveIterationResult;
        private readonly ICancelTokenReference _cancelTokenReference;

        public ExecuteTestsService
        (
            ClientOptions clientOptions,
            ILoadTestExecution loadTestExecution,
            IReadOnlyList<ISchedule> schedules,
            ISaveIterationResult saveIterationResult,
            ICancelTokenReference cancelTokenReference
        )
        {
            _clientOptions = clientOptions;
            _loadTestExecution = loadTestExecution;
            _schedules = schedules;
            _saveIterationResult = saveIterationResult;
            _cancelTokenReference = cancelTokenReference;
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

            return _loadTestExecution.Execute(_schedules, _cancelTokenReference.Token);
        }

        private static UserIterationFinished LogIteration(ISaveIterationResult iterationResultRepository)
        {
            return iterationResultRepository.Queue;
        }
    }
}
