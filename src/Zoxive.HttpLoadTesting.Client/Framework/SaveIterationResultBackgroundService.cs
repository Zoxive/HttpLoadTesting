using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Zoxive.HttpLoadTesting.Client.Domain.Database;
using Zoxive.HttpLoadTesting.Client.Domain.Iteration.Repositories;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Framework
{
    public class SaveIterationResultBackgroundService : BackgroundService, ISaveIterationResult
    {
        private readonly IIterationResultRepository _iterationResultRepository;
        private readonly string _name;
        private ConcurrentQueue<UserIterationResult> _queue = new ConcurrentQueue<UserIterationResult>();
        private Stopwatch _sw;

        public SaveIterationResultBackgroundService(IIterationResultRepository iterationResultRepository, string name)
        {
            _iterationResultRepository = iterationResultRepository;
            _name = name;
            _sw = new Stopwatch();
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
                    Console.WriteLine($"Writing {_queue.Count} into {_name}");
                    await SaveFromQueue(stoppingToken);
                }
            }
        }

        private async Task SaveFromQueue(CancellationToken stoppingToken, bool runAll = false)
        {
            if (!runAll)
                _sw.Reset();

            var count = _queue.Count;
            for (var i = 0; i < count; i++)
            {
                if (_queue.TryDequeue(out var result))
                {
                    try
                    {
                        await _iterationResultRepository.Save(result, stoppingToken);
                    }
                    catch (TaskCanceledException)
                    {
                        // Eat it
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{_name} - {e.Message}");
                    }

                    var elapsedMilliseconds = _sw.ElapsedMilliseconds;

                    if (runAll == false && elapsedMilliseconds > 2000)
                        break;

                    if (runAll && elapsedMilliseconds > 1000)
                    {
                        Console.WriteLine($"{_queue.Count} remaining..");
                    }
                }
            }

            if (!runAll)
                _sw.Stop();
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

    public class FileSaveIterationResult : SaveIterationResultBackgroundService
    {
        public FileSaveIterationResult(string databaseFile) : base(CreateFileRepository(databaseFile), nameof(FileSaveIterationResult))
        {
        }

        private static IterationResultRepository CreateFileRepository(string databaseFile)
        {
            var fileDb = new Db(new SqliteConnection($"Data Source={databaseFile};cache=shared"));
            var fileResultRepository = new IterationResultRepository(fileDb);
            DbInitializer.Initialize(fileDb);
            return fileResultRepository;
        }
    }
}