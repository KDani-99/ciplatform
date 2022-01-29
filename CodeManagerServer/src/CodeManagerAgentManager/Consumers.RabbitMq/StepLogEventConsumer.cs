using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CodeManager.Data.Events;
using CodeManager.Data.JsonWebTokens;
using CodeManager.Data.Repositories;
using CodeManagerAgentManager.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CodeManagerAgentManager.Consumers.RabbitMq
{
    [Obsolete("Use the streaming provided by SignalR instead")]
    public class StepLogEventConsumer : IConsumer<StepLogEvent>
    {
        private readonly ITokenService<JwtSecurityToken> _tokenService;
        private readonly IRunRepository _runRepository;
        private readonly IBusControl _busControl;
        private readonly ILogger<StepLogEventConsumer> _logger;
        
        public StepLogEventConsumer(ITokenService<JwtSecurityToken> tokenService, IRunRepository runRepository, IBusControl busControl, ILogger<StepLogEventConsumer> logger)
        {
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _runRepository = runRepository ?? throw new ArgumentNullException(nameof(runRepository));
            _busControl = busControl ?? throw new ArgumentNullException(nameof(busControl));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task Consume(ConsumeContext<StepLogEvent> context)
        {
            try
            {
                var claimPrincipal = await _tokenService.VerifyJobTokenAsync(context.Message.Token); // verify the token
                
                var runId = long.Parse(claimPrincipal.Claims.First(claim =>
                    claim.Type == CustomJwtRegisteredClaimNames.RunId).Value);
                var jobId = long.Parse(claimPrincipal.Claims.First(claim =>
                    claim.Type == CustomJwtRegisteredClaimNames.JobId).Value);
                
                await context.Publish(new VerifiedStepLogEvent
                {
                    RunId = runId,
                    JobId = jobId,
                    StepIndex = context.Message.StepIndex,
                    Line = context.Message.Line
                });
            }
            catch (Exception exception)
            {
                _logger.LogError($"Failed to consume `{nameof(StepLogEvent)}`. Error: {exception.StackTrace}");
            }
        }
    }
}