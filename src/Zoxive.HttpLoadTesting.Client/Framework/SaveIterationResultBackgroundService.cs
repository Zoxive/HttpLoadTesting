using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Zoxive.HttpLoadTesting.Client.Domain.Iteration.Repositories;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Framework
{
    public class SaveIterationResultBackgroundService : BackgroundService, ISaveIterationResult
    {
        private readonly IIterationResultRepository _iterationResultRepository;
        private readonly string _name;
        private ConcurrentQueue<UserIterationResult> _queue = new ConcurrentQueue<UserIterationResult>();

        public SaveIterationResultBackgroundService(IIterationResultRepository iterationResultRepository, string name)
        {
            _iterationResultRepository = iterationResultRepository;
            _name = name;
        }

        public void Queue(UserIterationResult result)
        {
            _queue.Enqueue(result);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var count = _queue.Count;
                if (count == 0)
                {
                    await Task.Delay(100, stoppingToken);
                }
                else
                {
                    await SaveFromQueue(stoppingToken);
                }
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

                if (_queue.TryDequeue(out var result))
                {
                    try
                    {
                        await _iterationResultRepository.Save(result);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{_name} - {e.Message}");
                    }

                    if (runAll && runAllStopwatch.ElapsedMilliseconds > 1000)
                    {
                        runAllStopwatch.Restart();
                        Console.WriteLine($"{_queue.Count} remaining..");
                    }
                }
            }

            runAllStopwatch?.Stop();
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);

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

    public interface ISaveIterationResult
    {
        void Queue(UserIterationResult result);
    }
}