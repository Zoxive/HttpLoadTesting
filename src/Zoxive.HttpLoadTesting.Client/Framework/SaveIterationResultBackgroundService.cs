using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Zoxive.HttpLoadTesting.Client.Domain.Database;
using Zoxive.HttpLoadTesting.Client.Domain.Iteration.Repositories;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Framework
{
    public class SaveIterationResultBackgroundService : BackgroundService, ISaveIterationResult
    {
        private readonly IIterationResultRepository _iterationResultRepository;
        private readonly string _databaseFile;
        private ConcurrentQueue<UserIterationResult> _queue = new ConcurrentQueue<UserIterationResult>();
        private readonly SqliteConnection _inmemoryConnection;

        public SaveIterationResultBackgroundService(IIterationResultRepository iterationResultRepository, SqliteConnection inmemoryConnection, string databaseFile)
        {
            _iterationResultRepository = iterationResultRepository;
            _inmemoryConnection = inmemoryConnection;
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

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            // TODO SAVE MEMORY DATABASE to file
            Console.WriteLine("Saving database to file...");
            
            var path = Path.Combine(Directory.GetCurrentDirectory(), _databaseFile);

            // touch file
            using (File.Create(path))
            {
            }

            await _inmemoryConnection.ExecuteAsync($"ATTACH '{path}' AS FILE");

            DbInitializer.Initialize(new Db(_inmemoryConnection), "FILE");

            var r = await _inmemoryConnection.ExecuteAsync("INSERT INTO FILE.Iteration SELECT * FROM main.Iteration");
            var rr = await _inmemoryConnection.ExecuteAsync("INSERT INTO FILE.HttpStatusResult SELECT * FROM main.HttpStatusResult");

            var t = await _inmemoryConnection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM File.Iteration");

            //await _inmemoryConnection.ExecuteAsync($"DETACH DATABASE FILE");

            _inmemoryConnection.Dispose();

            Console.WriteLine($"Done saving. {path} {t}");

            await base.StopAsync(cancellationToken);
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