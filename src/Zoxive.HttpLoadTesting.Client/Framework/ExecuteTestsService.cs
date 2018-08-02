using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
        private readonly ICancelTokenReference _cancelTokenReference;
        private readonly ILogger<ExecuteTestsService> _logger;

        public ExecuteTestsService
        (
            ClientOptions clientOptions,
            ILoadTestExecution loadTestExecution,
            IReadOnlyList<ISchedule> schedules,
            ISaveIterationQueue saveIterationQueue,
            ICancelTokenReference cancelTokenReference,
            ILogger<ExecuteTestsService> logger
        )
        {
            _clientOptions = clientOptions;
            _loadTestExecution = loadTestExecution;
            _schedules = schedules;
            _saveIterationQueue = saveIterationQueue;
            _cancelTokenReference = cancelTokenReference;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Using DatabaseFile: {DatabaseFile}", _clientOptions.DatabaseFile);

            // TODO allow user to say if viewing
            if (_clientOptions.Viewing)
            {
                _logger.LogInformation("Viewing Existing Data..");
                return Task.CompletedTask;
            }

            _logger.LogInformation("Running Tests..");

            _loadTestExecution.UserIterationFinished += LogIteration(_saveIterationQueue);

            return _loadTestExecution.Execute(_schedules, _cancelTokenReference.Token);
        }

        private static UserIterationFinished LogIteration(ISaveIterationQueue iterationQueueRepository)
        {
            return iterationQueueRepository.Queue;
        }
    }
}
