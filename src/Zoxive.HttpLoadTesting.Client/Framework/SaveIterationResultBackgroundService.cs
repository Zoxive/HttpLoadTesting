using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Zoxive.HttpLoadTesting.Client.Domain.Iteration.Repositories;
using Zoxive.HttpLoadTesting.Client.Framework.Core;

namespace Zoxive.HttpLoadTesting.Client.Framework
{
    public class SaveIterationResultBackgroundService : BackgroundService
    {
        private readonly ISimpleTransaction _transaction;
        private readonly IIterationResultRepository _iterationResultRepository;
        private ISaveIterationQueue _queue;
        private readonly string _name;

        public SaveIterationResultBackgroundService
        (
            ISimpleTransaction transaction,
            IIterationResultRepository iterationResultRepository,
            ISaveIterationQueue queue,
            string name
        )
        {
            _transaction = transaction;
            _iterationResultRepository = iterationResultRepository;
            _queue = queue;
            _name = name;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Execute(stoppingToken);
        }

        private async Task Execute(CancellationToken stoppingToken, bool runAll = false)
        {
            var tick = ValueStopwatch.StartNew();

            await _transaction.OpenConnection();

            var inserted = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                if (runAll && _queue.Count == 0)
                {
                    return;
                }

                inserted += await SaveFromQueue(stoppingToken);

                var timeElapsed = tick.GetElapsedTime().TotalSeconds > 1;
                if (timeElapsed)
                {
                    tick = ValueStopwatch.StartNew();

                    Console.WriteLine($"QueueSize: {_queue.Count}. Inserted {inserted}");

                    inserted = 0;
                }
            }
        }

        private async ValueTask<int> SaveFromQueue(CancellationToken stoppingToken)
        {
            if (stoppingToken.IsCancellationRequested)
                return 0;

            var result = await _queue.DequeueAsync(1500, stoppingToken);
            if (result == null || result.Count == 0) return 0;

            await _transaction.Begin();

            var i = 0;
            foreach (var item in result)
            {
                try
                {
                    await _iterationResultRepository.Save(item);
                    i++;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed {nameof(SaveFromQueue)} {_name}");
                    Console.WriteLine(e);
                }
            }

            await _transaction.Commit();

            return i;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);

            Console.WriteLine($"Attemping to stop {_name}");

            // Make sure we insert the remaining things
            if (_queue.Count > 0)
            {
                Console.WriteLine($"There are still items in queue {_name} {_queue.Count}");

                await Execute(default(CancellationToken), runAll: true);

                Console.WriteLine("Done.");
            }
        }
    }
}