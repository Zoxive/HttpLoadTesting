using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Zoxive.HttpLoadTesting.Client.Domain.Iteration.Repositories;

namespace Zoxive.HttpLoadTesting.Client.Framework
{
    public class SaveIterationResultBackgroundService : BackgroundService
    {
        private readonly IIterationResultRepository _iterationResultRepository;
        private ISaveIterationQueue _queue;
        private readonly string _name;

        public SaveIterationResultBackgroundService(IIterationResultRepository iterationResultRepository, ISaveIterationQueue queue, string name)
        {
            _iterationResultRepository = iterationResultRepository;
            _queue = queue;
            _name = name;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await SaveFromQueue(stoppingToken);
            }
        }

        private async Task SaveFromQueue(CancellationToken stoppingToken, bool runAll = false)
        {

            Stopwatch runAllStopwatch = null;
            if (runAll)
            {
                runAllStopwatch = new Stopwatch();
                runAllStopwatch.Restart();
            }

            var count = _queue.Count;
            for (var i = 0; i < count; i++)
            {
                if (stoppingToken.IsCancellationRequested)
                    return;

                var result = await _queue.DequeueAsync(stoppingToken);
                if (result != null)
                {
                    try
                    {
                        await _iterationResultRepository.Save(result);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Failed {nameof(SaveFromQueue)} {_name}");
                    }

                    if (runAll && runAllStopwatch.ElapsedMilliseconds > 1000)
                    {
                        runAllStopwatch.Restart();

                        Console.WriteLine($"QueueCount Remaining {_queue.Count}");
                    }
                }
            }

            runAllStopwatch?.Stop();
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);

            Console.WriteLine($"Attemping to stop {_name}");

            // Make sure we insert the remaining things
            if (_queue.Count > 0)
            {
                Console.WriteLine($"There are still items in queue {_name} {_queue.Count}");

                await SaveFromQueue(default(CancellationToken), runAll: true);

                Console.WriteLine("Done.");
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _queue = null;
        }
    }

   
}