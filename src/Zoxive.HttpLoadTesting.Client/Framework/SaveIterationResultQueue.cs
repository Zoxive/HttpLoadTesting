using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Framework
{
    public sealed class SaveIterationQueueQueue : ISaveIterationQueue, IDisposable
    {
        private readonly ConcurrentQueue<UserIterationResult> _queue = new ConcurrentQueue<UserIterationResult>();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);
        private bool _disposed;

        public void Queue(UserIterationResult result)
        {
            _queue.Enqueue(result);

            _signal.Release();
        }

        public int Count => _queue.Count;

        public async Task<IReadOnlyList<UserIterationResult>> DequeueAsync(int maxCount, CancellationToken cancellationToken)
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

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _signal.Dispose();
            _disposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
    }

    public interface ISaveIterationQueue
    {
        void Queue(UserIterationResult result);

        int Count { get; }

        Task<IReadOnlyList<UserIterationResult>> DequeueAsync(int maxCount, CancellationToken cancellationToken);
    }
}