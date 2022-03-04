using System;
using System.Threading.Tasks;
using Castle.Core.Logging;
using CIPlatform.Data.Commands;
using CIPlatform.Data.Entities;
using CIPlatformManager.Consumers;
using CIPlatformManager.Services.Runs;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CIPlatformManager.Tests.Consumers
{
    public class QueueRunCommandConsumerTests
    {
        [Test]
        public async Task Consume_ValidContext_ShouldCallQueueAsync()
        {
            // Arrange
            var message = new QueueRunCommand();
            var runService = new Mock<IRunService>();
            var logger = new Mock<ILogger<QueueRunCommandConsumer>>();
            
            var consumeContext = new Mock<ConsumeContext<QueueRunCommand>>();
            consumeContext.SetupGet(x => x.Message)
                .Returns(message);
            
            var queueRunCommandConsumer = new QueueRunCommandConsumer(runService.Object, logger.Object);

            // Act
            await queueRunCommandConsumer.Consume(consumeContext.Object);

            // Assert
            runService.Verify(x => x.QueueAsync(It.IsAny<QueueRunCommand>()));
        }
        
        [Test]
        public async Task Consume_InValidContext_ShouldRespondWithFailedQueueRunCommandResponse()
        {
            // Arrange
            var runService = new Mock<IRunService>();
            var logger = new Mock<ILogger<QueueRunCommandConsumer>>();
            
            var consumeContext = new Mock<ConsumeContext<QueueRunCommand>>();
            consumeContext.SetupGet(x => x.Message)
                .Throws(new ArgumentNullException(string.Empty));
            
            var queueRunCommandConsumer = new QueueRunCommandConsumer(runService.Object, logger.Object);

            // Act
            await queueRunCommandConsumer.Consume(consumeContext.Object);

            // Assert
            consumeContext.Verify(x => x.RespondAsync(It.IsAny<FailedQueueRunCommandResponse>()));
        }
    }
}