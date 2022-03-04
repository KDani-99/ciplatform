using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using CIPlatform.Data.Entities;
using CIPlatform.Data.Repositories;
using CIPlatformManager.Configuration;
using CIPlatformManager.Services.Logs;
using CIPlatformManager.SignalR;
using CIPlatformManager.Tests.Mock;
using CIPlatformWorker.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace CIPlatformManager.Tests.Services
{
    public class LogStreamServiceTests
    {
        [Test]
        public async Task ProcessStreamAsync_ValidDetails_ShouldSucceed()
        {
            // Arrange
            var pathstr = "C:\\test";
            var run = new RunEntity
            {
                Jobs = new List<JobEntity>
                {
                    new JobEntity()
                    {
                        Steps = new List<StepEntity>
                        {
                            new StepEntity()
                        }
                    }
                }
            };
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var logStreamServiceConfiguration = fixture.Create<IOptions<LogStreamServiceConfiguration>>();
            logStreamServiceConfiguration.Value.LogPath = pathstr;
            logStreamServiceConfiguration.Value.MaxFileSize = 10;
            logStreamServiceConfiguration.Value.MaxLinePerFile = 10;
            
            var runRepository = new Mock<IRunRepository>();
            runRepository.Setup(x => x.GetAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(run));

            var hubConnection = new Mock<MockHubConnection>();
            
            var managerClient = new Mock<IManagerClient>();
            managerClient.Setup(x => x.HubConnection)
                .Returns(hubConnection.Object);
            
            var directory = new Mock<IDirectory>();
            
            var path = new Mock<IPath>();
            path.Setup(x => x.GetFullPath(It.IsAny<string>()))
                .Returns(pathstr);
            path.Setup(x => x.Join(It.IsAny<string>()))
                .Returns(pathstr);
            
            await using var stream = new MemoryStream();
            
            var file = new Mock<IFile>();
            file.Setup(x =>
                    x.Open(It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>(), It.IsAny<FileShare>()))
                .Returns(stream);

            var fileSystem = new Mock<IFileSystem>();
            fileSystem.SetupGet(x => x.Path)
                .Returns(path.Object);
            fileSystem.SetupGet(x => x.File)
                .Returns(file.Object);
            fileSystem.SetupGet(x => x.Directory)
                .Returns(directory.Object);
            

            var runId = 0;
            var jobId = 0;
            var stepIndex = 0;

            var data = string.Empty;
            
            var channelReader = new Mock<ChannelReader<string>>();
            channelReader.SetupSequence(x => x.WaitToReadAsync(It.IsAny<CancellationToken>()))
                .Returns(ValueTask.FromResult(true))
                .Returns(ValueTask.FromResult(false));
            channelReader.SetupSequence(x => x.TryRead(out data))
                .Returns(true)
                .Returns(false);

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