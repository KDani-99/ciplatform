using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodeManager.Data.Configuration;
using CodeManager.Data.Events;
using CodeManager.Data.Extensions;
using CodeManager.Data.JsonWebTokens;
using CodeManagerAgent.Entities;
using CodeManagerAgent.Factories;
using CodeManagerAgent.Services;
using CodeManagerAgent.WebSocket;
using CodeManagerAgent.WebSocket.Consumers;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NUnit.Framework;

namespace CodeManagerAgent.Tests
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
            var queueJobCommand = new QueueJobCommand
            {
                Token = token
            };
            var jobHandlerServiceFactory = new Mock<IJobHandlerServiceFactory>();
            var logger = new Mock<ILogger<QueueJobCommandConsumer>>();
            var jobHandlerService = new Mock<IJobHandlerService>();
            var workerClient = new Mock<IWorkerClient>();

            jobHandlerServiceFactory
                .Setup(x => x.Create(It.IsAny<JobDetails>(), It.IsAny<JobConfiguration>(),
                                     It.IsAny<CancellationToken>()))
                .Returns(jobHandlerService.Object);

            var consumer = new QueueJobCommandConsumer(jobHandlerServiceFactory.Object,
                                                       logger.Object, workerClient.Object);

            // Act
            await consumer.ConsumeAsync(queueJobCommand);

            // Assert
            jobHandlerServiceFactory.Verify(x => x.Create(It.IsAny<JobDetails>(), It.IsAny<JobConfiguration>(),
                                                          It.IsAny<CancellationToken>()), Times.Exactly(1));
            jobHandlerService
                .Verify(x => x.PrepareEnvironmentAsync(), Times.Exactly(1));
            jobHandlerService
                .Verify(x => x.DisposeAsync(), Times.Exactly(1));
        }
    }
}