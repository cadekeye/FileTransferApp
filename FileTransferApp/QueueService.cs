using FileTransferApp.Interfaces;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileTransferApp
{
    public class QueueService : BackgroundService
    {
        private IBackgroundQueue _queue;

        public QueueService(IBackgroundQueue queue)
        {
            _queue = queue;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var task = await _queue.PopQueue(stoppingToken);

                await task(stoppingToken);
            }
        }
    }
}