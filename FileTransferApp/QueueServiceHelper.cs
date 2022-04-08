using FileTransferApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileTransferApp
{
    public class QueueServiceHelper : QueueService, IQueueServiceHelper
    {
        private readonly IBackgroundQueue backGroundQueue;

        public QueueServiceHelper(IBackgroundQueue _backGroundQueue) : base(_backGroundQueue)
        {
        }

        public async Task ExposeExecute(CancellationToken stoppingToken)
        {
            await base.ExecuteAsync(stoppingToken);
        }
    }
}