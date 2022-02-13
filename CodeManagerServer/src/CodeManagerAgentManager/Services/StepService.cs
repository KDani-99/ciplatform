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
using Microsoft.OpenApi.Extensions;

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
            
            await ProcessStepAsync(run, job, step, context.State);
            await ProcessJobAsync(runId, job, context.StepIndex, step.State);
            await ProcessRunAsync(run.Project.Id, run, step.State);

            await UpdateWorkerStateAsync(connectionId);

            await _runRepository.UpdateAsync(run);

            await SendStepResultNotificationAsync(jobId, step.Id, context.State, DateTime.Now); // TODO: do not send date if the state is Skipped
        }

        private async Task ProcessStepAsync(Run run, Job job, Step step, States state)
        {
            step.State = state;

            if (step.State is States.Successful or States.Failed) step.FinishedDateTime = DateTime.Now;

            switch (step.State)
            {
                case States.Failed:
                {
                    var collection = job.Steps.Where(x => x.Index > step.Index).ToList();
                    foreach (var notRunStep in collection)
                    {
                        notRunStep.State = States.Skipped; // mark the rest as skipped
                        await SendStepResultNotificationAsync(job.Id, notRunStep.Id, States.Skipped, null);
                    }

                    run.NumberOfCompletedSteps += collection.Count + 1;
                    break;
                }
                case States.Running:
                    step.StartedDateTime = DateTime.Now;
                    break;
                case States.Successful:
                case States.Cancelled:
                case States.Skipped:
                    run.NumberOfCompletedSteps += 1;
                    break;
                case States.NotRun:
                case States.Queued:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task ProcessJobAsync(long runId, Job job, int stepIndex, States state)
        {
            if (job.State is States.NotRun && state is States.Running)
            {
                await SendJobResultNotificationAsync(runId, job.Id, state, DateTime.Now);
            }
            
            job.State = state switch
            {
                States.Failed => States.Failed,
                States.Successful when stepIndex == job.Steps.Count - 1 => States.Successful,
                _ => job.State
            };

            if (job.State is States.Failed or States.Successful)
            {
                job.FinishedDateTime = DateTime.Now;
                await SendJobResultNotificationAsync(runId, job.Id, state, job.FinishedDateTime);
            }
        }

        private async Task ProcessRunAsync(long projectId, Run run, States state)
        {
            if (run.State is States.Queued && state is States.Running)
            {
                run.State = state;
                run.StartedDateTime = DateTime.Now;
                await SendRunResultNotificationAsync(projectId, run.Id, run.State, run.StartedDateTime);
            }

            if (state is States.Successful && IsLastStep(run) && run.State != States.Failed)
            {
                // make sure that it is the last step of the last job
                run.State = States.Successful;
                await SendRunResultNotificationAsync(projectId, run.Id, run.State, null);
            }
            
            if (run.State is not States.Failed && state is States.Failed)
            {
                run.State = state;
                await SendRunResultNotificationAsync(projectId, run.Id, run.State, null);
            }
            
            if (IsLastStep(run))
            {
                run.FinishedDateTime = DateTime.Now;
                await SendRunResultNotificationAsync(projectId, run.Id, run.State, run.FinishedDateTime);
            }
        }

        private async Task UpdateWorkerStateAsync(string connectionId)
        {
            var workerConnectionData = await _workerConnectionService.GetWorkerConnectionAsync(connectionId);
            workerConnectionData.AgentState = AgentState.Available;
            await _workerConnectionService.UpdateWorkerConnectionAsync(workerConnectionData);
        }

        private Task SendStepResultNotificationAsync(long jobId, long stepId, States state, DateTime? dateTime)
        {
            var processed = new ProcessedStepResultEvent
            {
                JobId = jobId,
                StepId = stepId,
                State = state,
                DateTime = dateTime
            };
            return _busControl.Publish(processed);
        }
        
        private Task SendJobResultNotificationAsync(long runId, long jobId, States state, DateTime? dateTime)
        {
            var processed = new ProcessedJobResultEvent
            {
                RunId = runId,
                JobId = jobId,
                State = state,
                DateTime = dateTime
            };
            return _busControl.Publish(processed);
        }
        
        private Task SendRunResultNotificationAsync(long projectId, long runId, States state, DateTime? dateTime)
        {
            var processed = new ProcessedRunResultEvent
            {
                ProjectId = projectId,
                RunId = runId,
                State = state,
                DateTime = dateTime
            };
            return _busControl.Publish(processed);
        }

        private static bool IsLastStep(Run run)
        {
            return run.NumberOfCompletedSteps == run.NumberOfSteps;
        }
    }
}