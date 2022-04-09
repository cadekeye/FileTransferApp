using FileTransferApp.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace FileTransferApp
{
    public class BackgroundQueue : IBackgroundQueue
    {
        private ConcurrentQueue<Func<CancellationToken, Task>> Tasks;
        private SemaphoreSlim Signals;

        public BackgroundQueue()
        {
            Tasks = new ConcurrentQueue<Func<CancellationToken, Task>>();
            Signals = new SemaphoreSlim(0);
        }

        public void QueueTask(Func<CancellationToken, Task> task)
        {
            Tasks.Enqueue(task);
            Signals.Release();
        }

        public async Task<Func<CancellationToken, Task>> PopQueue(CancellationToken cancellationToken)
        {
            await Signals.WaitAsync(cancellationToken);
            Tasks.TryDequeue(out var task);

            return task;
        }
    }
}