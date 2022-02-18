using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CIPlatform.Data.Agent;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Entities;
using CIPlatform.Data.Events;
using CIPlatform.Data.Extensions;
using CIPlatform.Data.JsonWebTokens;
using CIPlatform.Data.Repositories;
using CIPlatformManager.Entities;
using CIPlatformManager.Exceptions;
using CIPlatformManager.WebSocket.Hubs;
using IPlatformManager.WebSocket;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace CIPlatformManager.Services
{
    public class JobService : IJobService
    {
        private readonly IBusControl _busControl;
        private readonly IRunRepository _runRepository;
        private readonly ITokenService<JwtSecurityToken> _tokenService;
        private readonly IHubContext<WorkerHub, IWorkerClient> _hubContext;
        private readonly IWorkerConnectionService _workerConnectionService;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public JobService(IBusControl busControl, IRunRepository runRepository, ITokenService<JwtSecurityToken> tokenService, IHubContext<WorkerHub, IWorkerClient> hubContext, IWorkerConnectionService workerConnectionService, JsonSerializerOptions jsonSerializerOptions)
        {
            _busControl = busControl ?? throw new ArgumentNullException(nameof(busControl));
            _runRepository = runRepository ?? throw new ArgumentNullException(nameof(runRepository));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _workerConnectionService =
                workerConnectionService ?? throw new ArgumentException(nameof(workerConnectionService));
            _jsonSerializerOptions =
                jsonSerializerOptions ?? throw new ArgumentNullException(nameof(jsonSerializerOptions));
        }
        
        public async Task QueueJobAsync(CachedJobDetails jobDetails, string connectionId)
        {
            var run = await _runRepository.GetAsync(jobDetails.RunId) ?? throw new RunDoesNotExistException();

            await HandleFirstRunAsync(run, jobDetails.JobId);

            var job = run.Jobs.FirstOrDefault(j => j.Id == jobDetails.JobId) ?? throw new RunDoesNotExistException();
            
            var jobConfiguration =
                JsonSerializer.Deserialize<JobConfiguration>(job.JsonContext, _jsonSerializerOptions);

            var jobToken = await _tokenService.CreateJobTokenAsync(run.Id, job.Id);
            
            var queueJobCommand = new QueueJobCommand
            {
                Token = jobToken.ToBase64String(),
                JobConfiguration = jobConfiguration,
                Repository = run.Repository
            };

            // Remove from pool to make sure that other instances won't send event to that specific worker
            await UpdateWorkerStateAsync(connectionId);

            await _hubContext.Clients.Client(connectionId).QueueJob(queueJobCommand);
            
            job.State = States.Running; // TODO: -> remove it from here
            job.StartedDateTime = DateTime.Now;

            await SendJobNotificationAsync(run.Id, job.Id, States.Running);
            await _runRepository.UpdateAsync(run);
        }

        private async Task HandleFirstRunAsync(RunEntity run, long jobId)
        {
            if (run.Jobs.FindIndex(j => j.Id == jobId) == 0)
            {
                // If that's the first job of the given run
                // then change it's state to Queued and send update to clients
                run.State = States.Queued;
                run.StartedDateTime = DateTime.Now;
                await SendRunNotificationAsync(run.Id, States.Running);
            }
        }
        
        private Task SendJobNotificationAsync(long runId, long jobId, States state)
        {
            var processed = new ProcessedJobResultEvent
            {
                RunId = runId,
                JobId = jobId,
                State = state,
                DateTime = DateTime.Now
            };
            return _busControl.Publish(processed);
        }
        
        private Task SendRunNotificationAsync(long runId, States state)
        {
            var processed = new ProcessedRunResultEvent
            {
                RunId = runId,
                State = state,
                DateTime = DateTime.Now
            };
            return _busControl.Publish(processed);
        }
        
        private async Task UpdateWorkerStateAsync(string connectionId)
        {
            var workerConnectionData = await _workerConnectionService.GetWorkerConnectionAsync(connectionId);
            workerConnectionData.WorkerState = WorkerState.Working;
            await _workerConnectionService.UpdateWorkerConnectionAsync(workerConnectionData);
        }
    }
}