using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CIPlatform.Data.Agent;
using CIPlatform.Data.Commands;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Entities;
using CIPlatform.Data.Events;
using CIPlatform.Data.Extensions;
using CIPlatform.Data.Repositories;
using CIPlatformManager.Configuration;
using CIPlatformManager.Entities;
using CIPlatformManager.Exceptions;
using CIPlatformManager.Services.Jobs;
using CIPlatformManager.SignalR.Hubs;
using CIPlatformManager.SignalR;
using CIPlatformManager.WebSocket;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

namespace CIPlatformManager.Services.Runs
{
    public class RunService : IRunService
    {

        // Single unit of work, run and dispose
        private readonly IBusControl _busControl;
        private readonly IRunRepository _runRepository;
        private readonly IJobQueueService _jobQueueService;

        public RunService(IBusControl busControl,
                          IRunRepository runRepository,
                          IJobQueueService jobQueueService)
        {
            _busControl = busControl ?? throw new ArgumentNullException(nameof(busControl));
            _runRepository = runRepository ?? throw new ArgumentNullException(nameof(runRepository));
            _jobQueueService = jobQueueService ?? throw new ArgumentNullException(nameof(jobQueueService));
        }

        public async Task QueueAsync(QueueRunCommand cmd)
        {
            var run = await _runRepository.GetAsync(cmd.RunId);
            await SendRunQueuedNotification(run.Project.Id, run.Id);

            foreach (var job in run.Jobs)
                await QueueJobDetailsAsync(run.Id, job.Id, job.Context);
        }

        private Task QueueJobDetailsAsync(long runId, long jobId, JobContext jobContext)
        {
            var jobDetails = new CachedJobDetails
            {
                RunId = runId,
                JobId = jobId
            };

            return _jobQueueService.QueueAsync(jobDetails, jobContext);
        }

        private Task SendRunQueuedNotification(long projectId, long runId)
        {
            var processed = new ProcessedRunResultEvent
            {
                ProjectId = projectId,
                RunId = runId,
                State = States.Queued,
                DateTime = null
            };
            return _busControl.Publish(processed);
        }
    }
}