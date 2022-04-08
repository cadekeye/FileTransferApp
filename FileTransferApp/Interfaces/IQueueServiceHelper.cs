using System.Threading;
using System.Threading.Tasks;

namespace FileTransferApp
{
    public interface IQueueServiceHelper
    {
        Task ExposeExecute(CancellationToken stoppingToken);
    }
}