using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeManager.Core.Hubs.Common;
using CodeManager.Core.Hubs.Consumers;
using CodeManager.Data.Agent;
using CodeManager.Data.Commands;
using CodeManager.Data.Events;
using CodeManager.Data.Extensions;
using CodeManager.Data.JsonWebTokens;
using CodeManagerAgent.Entities;
using CodeManagerAgent.Factories;
using CodeManagerAgent.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace CodeManagerAgent.WebSocket.Consumers
{
    public class QueueJobCommandConsumer : IConsumer<QueueJobCommand>
    {
        private readonly IAgentService _agentService;
        private readonly IJobHandlerServiceFactory _jobHandlerServiceFactory;
        private readonly ILogger<QueueJobCommandConsumer> _logger;

        public QueueJobCommandConsumer(IAgentService agentService,
            IJobHandlerServiceFactory jobHandlerServiceFactory, ILogger<QueueJobCommandConsumer> logger)
        {
            _agentService = agentService ?? throw new ArgumentNullException(nameof(agentService));
            _jobHandlerServiceFactory = jobHandlerServiceFactory ??
                                        throw new ArgumentNullException(nameof(jobHandlerServiceFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ConsumeAsync(QueueJobCommand queueJobCommand, CancellationToken cancellationToken = default)
        {
            try
            {
                // TODO: only set agent state server side?
                _agentService.AgentState = AgentState.Working;

                await using var jobHandler = _jobHandlerServiceFactory.Create(GetJobDetails(queueJobCommand),
                    queueJobCommand.JobConfiguration,
                    cancellationToken);
                await jobHandler.StartAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError("An unexpected error has occurred. Error: " + exception.Message);
            }
        }

        private static JobDetails GetJobDetails(QueueJobCommand queueJobCommand)
        {
            var decodedToken = queueJobCommand.Token.DecodeJwtToken();
            var runId = long.Parse(decodedToken.Claims
                .FirstOrDefault(claim => claim.Type == CustomJwtRegisteredClaimNames.RunId)?.Value!);
            var jobId = long.Parse(decodedToken.Claims
                .FirstOrDefault(claim => claim.Type == CustomJwtRegisteredClaimNames.JobId)?.Value!);

            return new JobDetails
            {
                Token = queueJobCommand.Token,
                Repository = queueJobCommand.Repository,
                RunId = runId,
                JobId = jobId
            };
        }
    }
}