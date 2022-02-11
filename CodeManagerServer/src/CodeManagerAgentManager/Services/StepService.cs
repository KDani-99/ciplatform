using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using CodeManager.Data.Agent;
using CodeManager.Data.Configuration;
using CodeManager.Data.Entities;
using CodeManager.Data.Events;
using CodeManager.Data.JsonWebTokens;
using CodeManager.Data.Repositories;
using CodeManagerAgentManager.Exceptions;
using CodeManagerAgentManager.WebSocket;
using MassTransit;
using Microsoft.AspNetCore.SignalR.Client;

namespace CodeManagerAgentManager.Services
{
    public class StepService : IStepService<StepResultEvent>
    {
        private readonly IBusControl _busControl;
        private readonly IRunRepository _runRepository;
        private readonly ITokenService<JwtSecurityToken> _tokenService;
        private readonly IWorkerConnectionService _workerConnectionService;

        public StepService(IBusControl busControl,
                           IRunRepository runRepository,
                           ITokenService<JwtSecurityToken> tokenService,
                           IWorkerConnectionService workerConnectionService)
        {
            _busControl = busControl ?? throw new ArgumentNullException(nameof(busControl));
            _runRepository = runRepository ?? throw new ArgumentNullException(nameof(runRepository));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _workerConnectionService = workerConnectionService ??
                throw new ArgumentNullException(nameof(workerConnectionService));
        }

        public async Task ProcessStepResultAsync(StepResultEvent context, string connectionId)
        {
            var token = await _tokenService.VerifyJobTokenAsync(context.Token);

            var runId = long.Parse(token.Claims.First(claim => claim.Type == CustomJwtRegisteredClaimNames.RunId)
                                        .Value);
            var jobId = long.Parse(token.Claims.First(claim => claim.Type == CustomJwtRegisteredClaimNames.JobId)
                                        .Value);

            var run = await _runRepository.GetAsync(runId) ?? throw new RunDoesNotExistException();
            var job = run.Jobs.First(item => item.Id == jobId);

            var step = job.Steps.First(s => s.Index == context.StepIndex);
            
            ProcessStep(job, step, context.State);
            ProcessJob(job, context.StepIndex, step.State);
            ProcessRun(run, job, context.StepIndex, step.State);

            await UpdateWorkerStateAsync(connectionId);

            await _runRepository.UpdateAsync(run);

            await NotifyWebApi(run.Project.Id, run.Id, job.Id, context.StepIndex, context.State);
        }

        private static void ProcessStep(Job job, Step step, States state)
        {
            step.State = state;

            if (step.State is States.Successful or States.Failed) step.FinishedDateTime = DateTime.Now;

            switch (step.State)
            {
                case States.Failed:
                {
                    foreach(var notRunStep in job.Steps.Where(x => x.Index > step.Index))
                        notRunStep.State = States.Skipped; // mark the rest as skipped
                    break;
                }
                case States.Running:
                    step.StartedDateTime = DateTime.Now;
                    break;
                case States.NotRun:
                    break;
                case States.Successful:
                    break;
                case States.Cancelled:
                    break;
                case States.Skipped:
                    break;
                case States.Queued:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void ProcessJob(Job job, int stepIndex, States state)
        {
            job.State = state switch
            {
                States.Failed => States.Failed,
                States.Successful when stepIndex == job.Steps.Count - 1 => States.Successful,
                _ => job.State
            };
            
            if (job.State is not States.Running and not States.Queued) job.FinishedDateTime = DateTime.Now;
        }

        private static void ProcessRun(Run run, Job job, int stepIndex, States state)
        {
            run.State = state switch
            {
                States.Failed => States.Failed,
                States.Successful when stepIndex == job.Steps.Count - 1 => States.Successful,
                _ => run.State
            };

            if (run.State is not States.Running and not States.Queued) run.FinishedDateTime = DateTime.Now;
        }

        private async Task UpdateWorkerStateAsync(string connectionId)
        {
            var workerConnectionData = await _workerConnectionService.GetWorkerConnectionAsync(connectionId);
            workerConnectionData.AgentState = AgentState.Available;
            await _workerConnectionService.UpdateWorkerConnectionAsync(workerConnectionData);
        }

        private Task NotifyWebApi(long projectId, long runId, long jobId, long stepIndex, States state)
        {
            var processed = new ProcessedStepResult
            {
                StepIndex = stepIndex,
                JobId = jobId,
                RunId = runId,
                State = state,
                ProjectId = projectId
            };
            return _busControl.Publish(processed);
        }
    }
}