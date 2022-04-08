using FileTransferApp.Interfaces;
using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FileTransferApp;
using System.Threading;

namespace FileTransfer.Tests
{
    public class QueueServiceShould
    {
        private Mock<IBackgroundQueue> _backGroundQueueMock;
        private QueueServiceHelper queueService;

        public QueueServiceShould()
        {
            _backGroundQueueMock = new Mock<IBackgroundQueue>();
            queueService = new QueueServiceHelper(_backGroundQueueMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_WHEN_ISCancellationRequested_Verify0_PopQueue()
        {
            //Arrange
            CancellationTokenSource cancellationToken = new CancellationTokenSource();

            cancellationToken.Cancel();
            //Act
            await queueService.ExposeExecute(cancellationToken.Token);

            //Assert
            _backGroundQueueMock.Verify(x => x.PopQueue(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_WHEN_NOT_ISCancellationRequested_Verify1_PopQueue()
        {
            //Arrange
            CancellationTokenSource cancellationToken = new CancellationTokenSource();

            //Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => queueService.ExposeExecute(cancellationToken.Token));
            _backGroundQueueMock.Verify(x => x.PopQueue(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}