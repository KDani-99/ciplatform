using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using CodeManager.Core.Hubs.Common;
using CodeManager.Data.Configuration;
using CodeManager.Data.Entities.CI;
using CodeManager.Data.Events;
using CodeManagerAgent.Configuration;
using CodeManagerAgent.Entities;
using CodeManagerAgent.Exceptions;
using CodeManagerAgent.WebSocket;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeManagerAgent.Services
{
    public abstract class JobHandlerService : IJobHandlerService
    {
        private readonly AgentConfiguration _agentConfiguration;

        protected readonly CancellationToken CancellationToken;
        protected readonly JobConfiguration JobConfiguration;
        protected readonly JobDetails JobDetails;

        private bool _isEnvironmentPrepared;

        protected JobHandlerService(
            JobDetails jobDetails,
            JobConfiguration jobConfiguration,
            IOptions<AgentConfiguration> agentConfiguration,
            CancellationToken cancellationToken)
        {
            JobDetails = jobDetails ?? throw new ArgumentNullException(nameof(jobDetails));
            _agentConfiguration =
                agentConfiguration.Value ?? throw new ArgumentNullException(nameof(agentConfiguration));
            JobConfiguration = jobConfiguration ?? throw new ArgumentNullException(nameof(jobConfiguration));
            CancellationToken = cancellationToken;
        }

        public abstract ValueTask DisposeAsync();

        public abstract void Dispose();

        public virtual Task PrepareEnvironmentAsync()
        {
            JobConfiguration.Steps.Insert(0, new StepConfiguration
            {
                Name = "Checkout repository (setup)",
                Cmd = $"git clone {JobDetails.Repository} {_agentConfiguration.WorkingDirectory}"
            });

            _isEnvironmentPrepared = true;

            return Task.CompletedTask;
        }

        public virtual Task ExecuteStepAsync(ChannelWriter<string> channelWriter, StepConfiguration step, int stepIndex)
        {
            if (!_isEnvironmentPrepared)
            {
                throw new EnvironmentNotPreparedException("`PrepareEnvironmentAsync` must be called before executing any step.");
            }

            return Task.CompletedTask;
        }
    }
}