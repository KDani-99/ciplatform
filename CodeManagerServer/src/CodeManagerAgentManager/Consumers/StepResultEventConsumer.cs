using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Automatonymous;
using CodeManager.Data.Configuration;
using CodeManager.Data.Events;
using CodeManager.Data.JsonWebTokens;
using CodeManager.Data.Repositories;
using CodeManagerAgentManager.Exceptions;
using CodeManagerAgentManager.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CodeManagerAgentManager.Consumers
{
    public class StepResultEventConsumer : IConsumer<StepResultEvent>
    {
        private readonly IRunRepository _runRepository;
        private readonly ITokenService<JwtSecurityToken> _tokenService;
        private readonly ILogger<StepResultEventConsumer> _logger;

        public StepResultEventConsumer(IRunRepository runRepository, ITokenService<JwtSecurityToken> tokenService, ILogger<StepResultEventConsumer> logger)
        {
            _runRepository = runRepository ?? throw new ArgumentNullException(nameof(runRepository));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task Consume(ConsumeContext<StepResultEvent> context)
        {
            try
            {
                var token = await _tokenService.VerifyJobTokenAsync(context.Message.Token);
                
                var runId = long.Parse(token.Claims.First(claim => claim.Type == CustomJwtRegisteredClaimNames.RunId).Value);
                var jobId = long.Parse(token.Claims.First(claim => claim.Type == CustomJwtRegisteredClaimNames.JobId)
                    .Value);
                
                var run = await _runRepository.GetAsync(runId) ?? throw new RunDoesNotExistException();
                var job = run.Jobs.First(item => item.Id == jobId);

                var step = job.Steps[context.Message.StepIndex];
                step.State = context.Message.State;

                if (step.State is States.Successful or States.Failed)
                {
                    step.FinishedDateTime = DateTime.Now;
                }

                job.State = step.State switch
                {
                    States.Failed => States.Failed,
                    States.Successful when context.Message.StepIndex == job.Steps.Count - 1 => States.Successful,
                    _ => job.State
                };
                
                /*run.State = step.State switch
                {
                    States.Failed => States.Failed,
                    States.Successful when context.Message.StepIndex == job.Steps.Count - 1 => States.Successful,
                    _ => run.State
                };*/

                await _runRepository.UpdateAsync(run);
                // TODO: broadcast it to the client
                
            }
            catch (Exception exception)
            {
                _logger.LogError($"Failed to consume `{nameof(StepResultEvent)}`. Error: {exception.StackTrace}");
            }
        }
    }
}