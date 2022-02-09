using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Automatonymous;
using CodeManager.Data.Agent;
using CodeManager.Data.Configuration;
using CodeManager.Data.Entities;
using CodeManager.Data.Events;
using CodeManager.Data.JsonWebTokens;
using CodeManager.Data.Repositories;
using CodeManagerAgentManager.Exceptions;
using CodeManagerAgentManager.WebSocket;
using Microsoft.AspNetCore.SignalR.Client;

namespace CodeManagerAgentManager.Services
{
    public class StepService : IStepService<StepResultEvent>
    {
        private readonly IRunRepository _runRepository;
        private readonly ITokenService<JwtSecurityToken> _tokenService;
        private readonly IWorkerConnectionService _workerConnectionService;
        private readonly IManagerClient _managerClient;

        public StepService(IRunRepository runRepository, ITokenService<JwtSecurityToken> tokenService, IWorkerConnectionService workerConnectionService, IManagerClient managerClient)
        {
            _runRepository = runRepository ?? throw new ArgumentNullException(nameof(runRepository));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _workerConnectionService = workerConnectionService ??
                                       throw new ArgumentNullException(nameof(workerConnectionService));
            _managerClient = managerClient ?? throw new ArgumentNullException(nameof(managerClient));
        }
        
        public async Task ProcessStepResultAsync(StepResultEvent context, string connectionId)
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

            if (step.State == States.Failed)
            {
                // mark the rest as skipped
                foreach (var notRunStep in job.Steps.Skip(context.StepIndex + 1).TakeWhile((_) => true))
                {
                    notRunStep.State = States.Skipped;
                }
            }

            job.State = step.State switch
            {
                States.Failed => States.Failed,
                States.Successful when context.StepIndex == job.Steps.Count - 1 => States.Successful,
                _ => job.State
            };
                
            run.State = step.State switch
            {
                States.Failed => States.Failed,
                States.Successful when context.StepIndex == job.Steps.Count - 1 => States.Successful,
                _ => run.State
            };

            if (run.State is not States.Running and not States.Queued)
            {
                run.FinishedDateTime = DateTime.Now;
            }

            await UpdateWorkerStateAsync(connectionId);

            await _runRepository.UpdateAsync(run);

            await NotifyWebApi(context);
        }

        private async Task UpdateWorkerStateAsync(string connectionId)
        {
            var workerConnectionData = await _workerConnectionService.GetWorkerConnectionAsync(connectionId);
            workerConnectionData.AgentState = AgentState.Available;
            await _workerConnectionService.UpdateWorkerConnectionAsync(workerConnectionData);
        }
        
        private Task NotifyWebApi(StepResultEvent stepResultEvent)
        {
            var processed = new ProcessedStepResult
            {
                StepId = stepResultEvent.StepIndex,
                State = stepResultEvent.State
            };
            return _managerClient.HubConnection.SendAsync(CommonWebApiMethods.NotifyStepResult, processed);
        }
    }
}