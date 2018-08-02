using System.Collections.Concurrent;
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

        public async Task<UserIterationResult> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);

            _queue.TryDequeue(out var item);

            return item;
        }
    }

    public interface ISaveIterationQueue
    {
        void Queue(UserIterationResult result);

        int Count { get; }

        Task<UserIterationResult> DequeueAsync(CancellationToken cancellationToken);
    }
}