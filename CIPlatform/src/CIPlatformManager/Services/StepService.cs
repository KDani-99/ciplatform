using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using CIPlatform.Data.Agent;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Entities;
using CIPlatform.Data.Events;
using CIPlatform.Data.JsonWebTokens;
using CIPlatform.Data.Repositories;
using CIPlatformManager.Exceptions;
using CIPlatformManager.WebSocket;
using MassTransit;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.OpenApi.Extensions;

namespace CIPlatformManager.Services
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

            await _runRepository.UpdateAsync(run);
            
        }

        private async Task ProcessStepAsync(RunEntity run, JobEntity job, StepEntity step, States state)
        {
            step.State = state;
            
            await SendStepResultNotificationAsync(job.Id, step.Id, state, state is States.Skipped ? null : DateTime.Now);

            if (step.State is States.Successful or States.Failed) step.FinishedDateTime = DateTime.Now;
            
            switch (step.State)
            {
                case States.Failed:
                {
                    var numberOfSkippedSteps = await MarkStepsAsSkipped(job, step.Index);
                    run.NumberOfCompletedSteps += numberOfSkippedSteps + 1;
                    break;
                }
                case States.Running:
                    step.StartedDateTime = DateTime.Now;
                    break;
                case States.Successful:
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

        private async Task ProcessJobAsync(long runId, JobEntity job, int stepIndex, States state)
        {
            if (job.State is States.NotRun or States.Queued && state is States.Running)
            {
                job.State = state;
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

        private async Task ProcessRunAsync(long projectId, RunEntity run, States state)
        {
            if (run.State is States.Queued && state is States.Running)
            {
                run.State = state;
                run.StartedDateTime = DateTime.Now;
                await SendRunResultNotificationAsync(projectId, run.Id, run.State, run.StartedDateTime);
            }

            // make sure that it is the last step of the last job
            if (state is States.Successful && IsLastStep(run) && run.State != States.Failed)
            {
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

        private static bool IsLastStep(RunEntity run)
        {
            return run.NumberOfCompletedSteps == run.NumberOfSteps;
        }

        private async Task<int> MarkStepsAsSkipped(JobEntity job, int stepIndex)
        {
            var collection = job.Steps.Where(x => x.Index > stepIndex).ToList();
            foreach (var notRunStep in collection)
            {
                // mark the rest as skipped
                notRunStep.State = States.Skipped;
                await SendStepResultNotificationAsync(job.Id, notRunStep.Id, States.Skipped, null);
            }

            return collection.Count;
        }
    }
}