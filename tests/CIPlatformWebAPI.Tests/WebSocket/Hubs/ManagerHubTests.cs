using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Castle.Core.Logging;
using CIPlatformWebApi.WebSocket.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CIPlatformWebAPI.Tests.WebSocket.Hubs
{
    public class ManagerHubTests
    {
        [Test]
        public async Task StreamLogsToChannelAsync_NoErrors_ShouldSendAsync()
        {
            // Arrange
            var stepId = 0;
            var channelReader = new Mock<ChannelReader<string>>();
            
            channelReader.SetupSequence(x => x.WaitToReadAsync(It.IsAny<CancellationToken>()))
                .Returns(ValueTask.FromResult(true))
                .Returns(ValueTask.FromResult(false));
            
            var str = string.Empty;
            channelReader.SetupSequence(x => x.TryRead(out str))
                .Returns(true)
                .Returns(false);

            var hubContext = new Mock<IHubContext<RunsHub>>();
            
            var clientProxy = new Mock<IClientProxy>();
            
            var hubClients = new Mock<IHubClients>();
            hubClients.Setup(x => x.Group(It.IsAny<string>()))
                .Returns(clientProxy.Object);

            hubContext.SetupGet(x => x.Clients)
                .Returns(hubClients.Object);
            
            var logger = new Mock<ILogger<ManagerHub>>();
            
            var managerHub = new ManagerHub(logger.Object, hubContext.Object);
            
            // Act
            await managerHub.StreamLogsToChannelAsync(channelReader.Object, stepId);
            
            // Assert
            clientProxy.Verify(x => x.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(),It.IsAny<CancellationToken>()));
        }
    }
}