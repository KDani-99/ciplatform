using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using CodeManager.Data.Configuration;
using CodeManager.Data.Events;
using CodeManager.Data.JsonWebTokens;
using CodeManager.Data.Repositories;
using CodeManagerAgentManager.Exceptions;

namespace CodeManagerAgentManager.Services
{
    public class StepService : IStepService<StepResultEvent>
    {
        private readonly IRunRepository _runRepository;
        private readonly ITokenService<JwtSecurityToken> _tokenService;

        public StepService(IRunRepository runRepository, ITokenService<JwtSecurityToken> tokenService)
        {
            _runRepository = runRepository ?? throw new ArgumentNullException(nameof(runRepository));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }
        
        public async Task ProcessStepResultAsync(StepResultEvent context)
        {
            var token = await _tokenService.VerifyJobTokenAsync(context.Token);
                
            var runId = long.Parse(token.Claims.First(claim => claim.Type == CustomJwtRegisteredClaimNames.RunId).Value);
            var jobId = long.Parse(token.Claims.First(claim => claim.Type == CustomJwtRegisteredClaimNames.JobId)
                .Value);
                
            var run = await _runRepository.GetAsync(runId) ?? throw new RunDoesNotExistException();
            var job = run.Jobs.First(item => item.Id == jobId);

            var step = job.Steps[context.StepIndex];
            step.State = context.State;

            if (step.State is States.Successful or States.Failed)
            {
                step.FinishedDateTime = DateTime.Now;
            }

            job.State = step.State switch
            {
                States.Failed => States.Failed,
                States.Successful when context.StepIndex == job.Steps.Count - 1 => States.Successful,
                _ => job.State
            };
                
            /*run.State = step.State switch
            {
                States.Failed => States.Failed,
                States.Successful when context.Message.StepIndex == job.Steps.Count - 1 => States.Successful,
                _ => run.State
            };*/

            await _runRepository.UpdateAsync(run);
        }
    }
}