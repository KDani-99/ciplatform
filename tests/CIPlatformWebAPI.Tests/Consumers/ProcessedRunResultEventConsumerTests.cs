using System.Threading;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.Consumers;
using CIPlatformWebApi.WebSocket.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.Framework;

namespace CIPlatformWebAPI.Tests.Consumers
{
    public class ProcessedRunResultEventConsumerTests
    {
        [Test]
        public async Task Consume_NoErrors_ShouldSendAsync()
        {
            // Arrange
            var message = new ProcessedRunResultEvent();
            
            var consumeContext = new Mock<ConsumeContext<ProcessedRunResultEvent>>();
            consumeContext.SetupGet(x => x.Message)
                .Returns(message);
            
            var runsHubContext = new Mock<IHubContext<RunsHub>>();
            
            var clientProxy = new Mock<IClientProxy>();
            
            var hubClients = new Mock<IHubClients>();
            hubClients.Setup(x => x.Group(It.IsAny<string>()))
                .Returns(clientProxy.Object);

            runsHubContext.SetupGet(x => x.Clients)
                .Returns(hubClients.Object);
            
            var processedRunResultEventConsumer = new ProcessedRunResultEventConsumer(runsHubContext.Object);
            
            // Act
            await processedRunResultEventConsumer.Consume(consumeContext.Object);

            // Assert
            clientProxy.Verify(x => x.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(),It.IsAny<CancellationToken>()));
        }
    }
}