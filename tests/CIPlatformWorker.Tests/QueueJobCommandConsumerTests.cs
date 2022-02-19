using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CIPlatformWorker.Entities;
using CIPlatformWorker.Services;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Events;
using CIPlatform.Data.Extensions;
using CIPlatform.Data.JsonWebTokens;
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
            var queueJobCommand = new QueueJobCommand
            {
                Token = token,
                JobConfiguration = new JobConfiguration
                {
                    Context = JobContext.Docker
                }
            };
            var logger = new Mock<ILogger<QueueJobCommandConsumer>>();
            var jobHandlerService = new Mock<IJobHandlerService>();
            var workerClient = new Mock<IWorkerClient>();
            

            var consumer = new QueueJobCommandConsumer(, logger.Object, workerClient.Object, jobHandlerService.Object);

            // Act
            await consumer.ConsumeAsync(queueJobCommand);

            // Assert
            jobHandlerService
                .Verify(x => x.PrepareEnvironmentAsync(), Times.Exactly(1));
        }
    }
}