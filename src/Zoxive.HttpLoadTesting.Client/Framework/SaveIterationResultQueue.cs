using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Framework
{
    public class SaveIterationQueueQueue : ISaveIterationQueue
    {
        private readonly ConcurrentQueue<UserIterationResult> _queue = new ConcurrentQueue<UserIterationResult>();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void Queue(UserIterationResult result)
        {
            _queue.Enqueue(result);

            _signal.Release();
        }

        public int Count => _queue.Count;

        public async Task<IReadOnlyList<UserIterationResult>> DequeueAsync(CancellationToken cancellationToken, int maxCount)
        {
            await _signal.WaitAsync(cancellationToken);

            var i = 0;
            var list = new List<UserIterationResult>(maxCount);
            while (_queue.TryDequeue(out var item) && i++ < maxCount)
            {
                list.Add(item);
            }

            return list;
        }
    }

    public interface ISaveIterationQueue
    {
        void Queue(UserIterationResult result);

        int Count { get; }

        Task<IReadOnlyList<UserIterationResult>> DequeueAsync(CancellationToken cancellationToken, int maxCount);
    }
}