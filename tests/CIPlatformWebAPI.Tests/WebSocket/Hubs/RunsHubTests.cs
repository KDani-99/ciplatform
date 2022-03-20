using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.Exceptions;
using CIPlatformWebApi.Strategies;
using CIPlatformWebApi.WebSocket.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.Framework;

namespace CIPlatformWebAPI.Tests.WebSocket.Hubs
{
    public class RunsHubTests
    {
        [Test]
        public async Task SubscribeToResultsChannelAsync_NoErrors_ShouldAddToGroupAsync()
        {
            // Arrange
            var channel = "project";
            var entityId = 0;
            
            var resultChannelConnectionHandler = new Mock<IResultChannelConnectionHandler>();
            resultChannelConnectionHandler.Setup(x => x.VerifyAsync(It.IsAny<long>(), It.IsAny<UserEntity>()))
                .Returns(Task.FromResult(true));
            
            var iResultChannelConnectionHandlerFactory = new Mock<IResultChannelConnectionHandlerFactory>();
            iResultChannelConnectionHandlerFactory.Setup(x => x.Create(It.IsAny<string>()))
                .Returns(resultChannelConnectionHandler.Object);

            var items = new Dictionary<object, object?>()
            {
                {"user", new UserEntity()}
            };
            
            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(x => x.Items)
                .Returns(items);
            
            var httpContextFeature = new Mock<IHttpContextFeature>();
            httpContextFeature.SetupGet(x => x.HttpContext)
                .Returns(httpContext.Object);
            
            var features = new Mock<IFeatureCollection>();
            features.Setup(x => x.Get<IHttpContextFeature>())
                .Returns(httpContextFeature.Object);
            
            var hubCallerContext = new Mock<HubCallerContext>();
            hubCallerContext.SetupGet(x => x.Features)
                .Returns(features.Object);

            var groupManager = new Mock<IGroupManager>();
                
            var runsHub = new RunsHub(iResultChannelConnectionHandlerFactory.Object);
            runsHub.Context = hubCallerContext.Object;
            runsHub.Groups = groupManager.Object;
            
            // Act
            await runsHub.SubscribeToResultsChannelAsync(channel, entityId);

            // Assert
            iResultChannelConnectionHandlerFactory.Verify(x => x.Create(It.IsAny<string>()));
            groupManager.Verify(x => x.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));
        }
        
        [Test]
        public void SubscribeToResultsChannelAsync_InvalidUser_ShouldThrowUnauthorizedAccessWebException()
        {
            // Arrange
            var channel = "project";
            var entityId = 0;
            
            var resultChannelConnectionHandler = new Mock<IResultChannelConnectionHandler>();
            resultChannelConnectionHandler.Setup(x => x.VerifyAsync(It.IsAny<long>(), It.IsAny<UserEntity>()))
                .Returns(Task.FromResult(false));
            
            var iResultChannelConnectionHandlerFactory = new Mock<IResultChannelConnectionHandlerFactory>();
            iResultChannelConnectionHandlerFactory.Setup(x => x.Create(It.IsAny<string>()))
                .Returns(resultChannelConnectionHandler.Object);

            var items = new Dictionary<object, object?>()
            {
                {"user", new UserEntity()}
            };
            
            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(x => x.Items)
                .Returns(items);
            
            var httpContextFeature = new Mock<IHttpContextFeature>();
            httpContextFeature.SetupGet(x => x.HttpContext)
                .Returns(httpContext.Object);
            
            var features = new Mock<IFeatureCollection>();
            features.Setup(x => x.Get<IHttpContextFeature>())
                .Returns(httpContextFeature.Object);
            
            var hubCallerContext = new Mock<HubCallerContext>();
            hubCallerContext.SetupGet(x => x.Features)
                .Returns(features.Object);

            var groupManager = new Mock<IGroupManager>();
                
            var runsHub = new RunsHub(iResultChannelConnectionHandlerFactory.Object);
            runsHub.Context = hubCallerContext.Object;
            runsHub.Groups = groupManager.Object;
            
            // Act and Assert
            Assert.ThrowsAsync<UnauthorizedAccessWebException>(() =>
                runsHub.SubscribeToResultsChannelAsync(channel, entityId));
        }
        
        [Test]
        public async Task UnSubscribeToResultsChannelAsync_NoErrors_ShouldRemoveFromGroupAsync()
        {
            // Arrange
            var channel = "project";
            var entityId = 0;
            var connectionId = "abc-123";
            
            var resultChannelConnectionHandler = new Mock<IResultChannelConnectionHandler>();
            resultChannelConnectionHandler.Setup(x => x.VerifyAsync(It.IsAny<long>(), It.IsAny<UserEntity>()))
                .Returns(Task.FromResult(true));
            
            var iResultChannelConnectionHandlerFactory = new Mock<IResultChannelConnectionHandlerFactory>();

            var groupManager = new Mock<IGroupManager>();
            
            var hubCallerContext = new Mock<HubCallerContext>();
            hubCallerContext.SetupGet(x => x.ConnectionId)
                .Returns(connectionId);
            
            var runsHub = new RunsHub(iResultChannelConnectionHandlerFactory.Object);
            runsHub.Context = hubCallerContext.Object;
            runsHub.Groups = groupManager.Object;
            
            // Act
            await runsHub.UnSubscribeFromResultsChannelAsync(channel, entityId);

            // Assert
            groupManager.Verify(x => x.RemoveFromGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));
        }
        
        [Test]
        public void UnSubscribeToResultsChannelAsync_InvalidChannel_ShouldThrowChannelDoesNotExistException()
        {
            // Arrange
            var channel = "invalid";
            var entityId = 0;

            var resultChannelConnectionHandler = new Mock<IResultChannelConnectionHandler>();
            resultChannelConnectionHandler.Setup(x => x.VerifyAsync(It.IsAny<long>(), It.IsAny<UserEntity>()))
                .Returns(Task.FromResult(true));
            
            var iResultChannelConnectionHandlerFactory = new Mock<IResultChannelConnectionHandlerFactory>();

            var groupManager = new Mock<IGroupManager>();
            
            
            var runsHub = new RunsHub(iResultChannelConnectionHandlerFactory.Object);
            runsHub.Groups = groupManager.Object;
            
            // Act and Assert
            Assert.ThrowsAsync<ChannelDoesNotExistException>(() =>
                runsHub.UnSubscribeFromResultsChannelAsync(channel, entityId));
        }
    }
}