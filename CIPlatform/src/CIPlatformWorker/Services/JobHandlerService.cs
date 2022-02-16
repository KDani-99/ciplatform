using System;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using CIPlatformWorker.Configuration;
using CIPlatformWorker.Entities;
using CIPlatformWorker.Exceptions;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Events;
using Microsoft.Extensions.Options;

namespace CIPlatformWorker.Services
{
    public abstract class JobHandlerService : IJobHandlerService
    {
        private readonly AgentConfiguration _agentConfiguration;
        private readonly JobDetails _jobDetails;

        protected readonly CancellationToken CancellationToken;
        protected readonly JobConfiguration JobConfiguration;

        private bool _isEnvironmentPrepared;
        private int _lineNo = 0;

        protected JobHandlerService(JobDetails jobDetails,
                                    JobConfiguration jobConfiguration,
                                    IOptions<AgentConfiguration> agentConfiguration,
                                    CancellationToken cancellationToken)
        {
            _jobDetails = jobDetails ?? throw new ArgumentNullException(nameof(jobDetails));
            _agentConfiguration =
                agentConfiguration.Value ?? throw new ArgumentNullException(nameof(agentConfiguration));
            JobConfiguration = jobConfiguration ?? throw new ArgumentNullException(nameof(jobConfiguration));
            CancellationToken = cancellationToken;
        }

        public abstract ValueTask DisposeAsync();

        public abstract void Dispose();

        public virtual Task PrepareEnvironmentAsync()
        {
            var initialStep = JobConfiguration.Steps.First();
            initialStep.Cmd = initialStep.Cmd.Replace("[wd]", _agentConfiguration.WorkingDirectory);

            _isEnvironmentPrepared = true;

            return Task.CompletedTask;
        }

        public virtual Task ExecuteStepAsync(ChannelWriter<string> channelWriter, StepConfiguration step, int stepIndex)
        {
            if (!_isEnvironmentPrepared)
                throw new EnvironmentNotPreparedException(
                    "`PrepareEnvironmentAsync` must be called before executing any step.");

            return Task.CompletedTask;
        }
    }
}