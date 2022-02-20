using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using CIPlatformWorker.Entities;
using CIPlatformWorker.Services;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Events;
using CIPlatform.Data.Extensions;
using CIPlatform.Data.JsonWebTokens;
using CIPlatformWorker.Factories.Job;
using CIPlatformWorker.Services.Job;
using CIPlatformWorker.SignalR;
using CIPlatformWorker.SignalR.Consumers;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NUnit.Framework;

namespace CIPlatformWorker.Tests
{
    public class QueueJobCommandConsumerTests
    {
        private JwtSecurityToken CreateMockJwtToken(IEnumerable<Claim> claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretBytes = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(1),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(secretBytes),
                                           SecurityAlgorithms.HmacSha256Signature)
            };

            return tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        }

        [Test]
        public async Task ReceiveAsync_ConsumeReceivedMessage_ShouldProcessJob()
        {
            // Arrange
            var token = CreateMockJwtToken(new[]
            {
                new Claim(CustomJwtRegisteredClaimNames.JobId, "0"),
                new Claim(CustomJwtRegisteredClaimNames.RunId, "0")
            }).ToBase64String();
            var numberOfSteps = 5;
            var queueJobCommand = new QueueJobCommand
            {
                Token = token,
                JobConfiguration = new JobConfiguration
                {
                    Context = JobContext.Docker,
                    Steps = Enumerable.Repeat(new StepConfiguration(), numberOfSteps).ToList()
                }
            };
            var logger = new Mock<ILogger<QueueJobCommandConsumer>>();
            var jobHandlerService = new Mock<IJobHandlerService>();
            var workerClient = new Mock<IWorkerClient>();
            var jobHandlerServiceFactory = new Mock<IJobHandlerServiceFactory>();
            jobHandlerServiceFactory
                .Setup(x => x.Create(It.IsAny<JobConfiguration>(), It.IsAny<CancellationToken>()))
                .Returns(jobHandlerService.Object);

            var consumer = new QueueJobCommandConsumer(jobHandlerServiceFactory.Object ,logger.Object, workerClient.Object);

            // Act
            await consumer.ConsumeAsync(queueJobCommand);

            // Assert
            jobHandlerServiceFactory
                .Verify(x => x.Create(It.IsAny<JobConfiguration>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
            jobHandlerService
                .Verify(x => x.PrepareEnvironmentAsync(), Times.Exactly(1));
            jobHandlerService
                .Verify(x => x.ExecuteStepAsync(It.IsAny<ChannelWriter<string>>(), It.IsAny<StepConfiguration>(), It.IsAny<int>()), Times.Exactly(5));
        }
    }
}