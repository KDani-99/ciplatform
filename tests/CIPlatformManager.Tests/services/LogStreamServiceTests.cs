using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Channels;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using CIPlatform.Data.Entities;
using CIPlatform.Data.Repositories;
using CIPlatformManager.Configuration;
using CIPlatformManager.Services.Logs;
using CIPlatformManager.SignalR;
using CIPlatformWorker.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace CIPlatformManager.Tests
{
    public class LogStreamServiceTests
    {
        [Test]
        public async Task Test1()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var logStreamServiceConfiguration = fixture.Create<IOptions<LogStreamServiceConfiguration>>();
            logStreamServiceConfiguration.Value.LogPath = "C:\\test";
            var runRepository = new Mock<IRunRepository>();
            var managerClient = new Mock<IManagerClient>();
            var directory = new Mock<IDirectory>();
            var path = new Mock<IPath>();
            var file = new Mock<IFile>();
            var fileSystem = new Mock<MockFileSystem>();

            var runId = 0;
            var jobId = 0;
            var stepIndex = 0;
            var channelReader = new Mock<ChannelReader<string>>();
            var logStreamService = new LogStreamService(logStreamServiceConfiguration, runRepository.Object,
                managerClient.Object, fileSystem.Object);
            // Act
            await logStreamService.ProcessStreamAsync(channelReader.Object, runId, jobId, stepIndex);
            // Assert
            directory.Verify(x => x.CreateDirectory(It.IsAny<string>()), Times.Exactly(1));
            runRepository.Verify(x => x.GetAsync(It.IsAny<long>()), Times.Exactly(1));
            runRepository.Verify(x => x.UpdateAsync(It.IsAny<RunEntity>()), Times.Exactly(1));
        }
    }
}