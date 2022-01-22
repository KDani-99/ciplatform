using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CodeManager.Data.Commands;
using CodeManager.Data.Configuration;
using CodeManager.Data.Events;
using CodeManager.Data.Extensions;
using CodeManager.Data.JsonWebTokens;
using CodeManager.Data.Repositories;
using CodeManagerAgentManager.Exceptions;
using CodeManagerAgentManager.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CodeManagerAgentManager.Consumers
{
    public class RequestJobCommandConsumer : IConsumer<RequestJobCommand>
    {
        private readonly IRunRepository _runRepository;
        private readonly ITokenService<JwtSecurityToken> _tokenService;
        private readonly ILogger<StepResultEventConsumer> _logger;
        public RequestJobCommandConsumer(IRunRepository runRepository, ITokenService<JwtSecurityToken> tokenService, ILogger<StepResultEventConsumer> logger)
        {
            _runRepository = runRepository ?? throw new ArgumentNullException(nameof(runRepository));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task Consume(ConsumeContext<RequestJobCommand> context)
        {
            // Received from a connected worker (agent)
            // RPC

            try
            {
                var token = await _tokenService.VerifyJobRequestTokenAsync(context.Message.Token);
                var runId = long.Parse(token.Claims.First(claim => claim.Type == CustomJwtRegisteredClaimNames.RunId).Value);
                var jobId = long.Parse(token.Claims.First(claim => claim.Type == CustomJwtRegisteredClaimNames.JobId)
                    .Value);

                var run = await _runRepository.GetAsync(runId) ?? throw new RunDoesNotExistException();

                if (run.State == States.Failed)
                {
                    // TODO: run requires different states, it cant be "Skipped"
                    throw new RunFailedException();
                }
                
                var job = run.Jobs.First(item => item.Id == jobId);

                if (job.State != States.Queued)
                {
                    throw new RunAlreadyStartedException(); // job already started by a different consumer
                }
                
                if (run.State != States.Running)
                {
                    run.State = States.Running;
                }
                
                job.State = States.Running;
                job.StartDateTime = DateTime.Now;
                
                await _runRepository.UpdateAsync(run);

                var jobToken = await _tokenService.CreateJobTokenAsync(runId, jobId);

                await context.RespondAsync(new AcceptedRequestJobCommandResponse
                {
                    Token = jobToken.ToBase64String(),
                    JobConfiguration = JsonSerializer.Deserialize<JobConfiguration>(job.JsonContext)
                });

            }
            catch (Exception exception)
            {
                await context.RespondAsync(new RejectedRequestJobCommandResponse());
                _logger.LogError($"Failed to consume `{nameof(RequestJobCommand)}`. Error: {exception.Message}");
            }
        }
    }
}