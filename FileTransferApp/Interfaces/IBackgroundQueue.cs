using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileTransferApp.Interfaces
{
    public interface IBackgroundQueue
    {
        void QueueTask(Func<CancellationToken, Task> task);

        Task<Func<CancellationToken, Task>> PopQueue(CancellationToken cancellationToken);
    }
}
