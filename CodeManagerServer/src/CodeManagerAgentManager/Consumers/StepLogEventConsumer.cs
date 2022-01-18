using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using CodeManager.Data.Events;
using CodeManager.Data.JsonWebTokens;
using CodeManager.Data.Repositories;
using CodeManagerAgentManager.Services;
using MassTransit;

namespace CodeManagerAgentManager.Consumers
{
    public class StepLogEventConsumer : IConsumer<StepLogEvent>
    {
        private readonly ITokenService<JwtSecurityToken> _tokenService;
        private readonly IRunRepository _runRepository;
        private readonly IBusControl _busControl;
        
        public StepLogEventConsumer(ITokenService<JwtSecurityToken> tokenService, IRunRepository runRepository, IBusControl busControl)
        {
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _runRepository = runRepository ?? throw new ArgumentNullException(nameof(runRepository));
            _busControl = busControl ?? throw new ArgumentNullException(nameof(busControl));
        }
        
        public async Task Consume(ConsumeContext<StepLogEvent> context)
        {
            // TODO: middleware validation for messages?? via a rabbitmq service provider
            // TODO: calling the database for each log might be expensive => cache logs
            // TODO: => it doesnt even require DB call, stream it back to a different queue, back to WebAPI
            // TODO: encode send endpoint in JWT
            try
            {
                var claimPrincipal = await _tokenService.VerifyJobTokenAsync(context.Message.Token); // verify the token
                var sendEndpointAddress =
                    claimPrincipal.Claims.First(claim => claim.Type == CustomJwtRegisteredClaimNames.SendEndpoint).Value;
                
                // TODO: validate token only
                // maybe encrypt the endpoint?
              /* 
                // if the token is valid, update job
                var runId = long.Parse(claimPrincipal.Claims.First(claim => claim.Type == CustomJwtRegisteredClaimNames.RunId).Value);

                var runCtx = (await _runRepository.GetAsync(run => run.Id == runId)).First();*/

                var sendEndpoint = await _busControl.GetSendEndpoint(new Uri(sendEndpointAddress)); // get web api requester address
                await sendEndpoint.Send<>()
                    
                    --
            }
            catch
            {
                
            }
            throw new System.NotImplementedException();
        }
    }
}