using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Client.Domain.Iteration.Repositories;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Framework
{
    public class SaveIterationResultBackgroundService : BackgroundService, ISaveIterationResult
    {
        private readonly IIterationResultRepository _iterationResultRepository;
        private ConcurrentQueue<UserIterationResult> _queue = new ConcurrentQueue<UserIterationResult>();

        public SaveIterationResultBackgroundService(IIterationResultRepository iterationResultRepository)
        {
            _iterationResultRepository = iterationResultRepository;
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