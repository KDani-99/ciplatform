using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CodeManager.Data.Commands;
using CodeManager.Data.Configuration;
using CodeManager.Data.Entities;
using CodeManager.Data.Events;
using CodeManager.Data.Extensions;
using CodeManager.Data.JsonWebTokens;
using CodeManager.Data.Repositories;
using CodeManagerAgentManager.Exceptions;
using CodeManagerAgentManager.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CodeManagerAgentManager.Consumers.RabbitMq
{
    /*public class RequestJobCommandConsumer : IConsumer<RequestJobCommand>
    { // cant be made abstract is it is being called by rmq
        private readonly IJobService<AcceptedRequestJobCommandResponse> _jobService;
        private readonly ILogger<StepResultEventConsumer> _logger;

        public RequestJobCommandConsumer(IJobService<AcceptedRequestJobCommandResponse> jobService, ILogger<StepResultEventConsumer> logger)
        {
            _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task Consume(ConsumeContext<RequestJobCommand> context)
        {
            // Received from a connected worker (agent)
            // RPC
            try
            {
                var jobParams = await _jobService.ProcessJobRequestTokenAsync(context.Message.Token);
                await context.RespondAsync(jobParams);
            }
            catch (Exception exception)
            {
                await context.RespondAsync(new RejectedRequestJobCommandResponse());
                _logger.LogError($"Failed to consume `{nameof(RequestJobCommand)}`. Error: {exception.Message}");
            }
        }
    }*/
}