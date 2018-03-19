using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Zoxive.HttpLoadTesting.Client.Domain.Iteration.Repositories;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Framework
{
    public class SaveIterationResultBackgroundService : BackgroundService, ISaveIterationResult
    {
        private readonly IIterationResultRepository _iterationResultRepository;
        private readonly SqliteConnection _readerConnection;
        private readonly string _databaseFile;
        private ConcurrentQueue<UserIterationResult> _queue = new ConcurrentQueue<UserIterationResult>();

        public SaveIterationResultBackgroundService(IIterationResultRepository iterationResultRepository, SqliteConnection readerConnection, string databaseFile)
        {
            _iterationResultRepository = iterationResultRepository;
            _readerConnection = readerConnection;
            _databaseFile = databaseFile;
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
                    for (var i = 0; i < count; i++)
                    {
                        if (_queue.TryDequeue(out var result))
                        {
                            await _iterationResultRepository.Save(result);
                        }
                    }
                }
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            // TODO SAVE MEMORY DATABASE to file
            Console.WriteLine("Saving database to file...");

            return base.StopAsync(cancellationToken);
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