using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.Services.Run;
using CIPlatformWebApi.Strategies;
using Moq;
using NUnit.Framework;

namespace CIPlatformWebAPI.Tests.WebSocket.ConnectionHandlers
{
    public class StepResultChannelConnectionHandlerTests
    {
        [Test]
        public async Task VerifyAsync_Allowed_ShouldReturnTrue()
        {
            // Arrange
            var entityId = 0;
            var userEntity = new UserEntity();
            
            var runService = new Mock<IRunService>();
            runService.Setup(x => x.IsAllowedStep(It.IsAny<long>(), It.IsAny<UserEntity>()))
                .Returns(Task.FromResult(true));
            
            var stepResultChannelConnectionHandler = new StepResultChannelConnectionHandler(runService.Object);

            // Act
            var result = await stepResultChannelConnectionHandler.VerifyAsync(entityId, userEntity);

            // Assert
            Assert.True(result);
            runService.Verify(x => x.IsAllowedStep(It.IsAny<long>(), It.IsAny<UserEntity>()));
        }
    }
}