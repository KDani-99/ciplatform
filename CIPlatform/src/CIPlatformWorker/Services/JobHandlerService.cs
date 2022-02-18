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
        private readonly WorkerConfiguration _workerConfiguration;

        protected readonly CancellationToken CancellationToken;
        protected readonly JobConfiguration JobConfiguration;

        private bool _isEnvironmentPrepared;
        private int _lineNo = 0;

        protected JobHandlerService(
                                    JobConfiguration jobConfiguration,
                                    IOptions<WorkerConfiguration> agentConfiguration,
                                    CancellationToken cancellationToken)
        {
            _workerConfiguration =
                agentConfiguration.Value ?? throw new ArgumentNullException(nameof(agentConfiguration));
            JobConfiguration = jobConfiguration ?? throw new ArgumentNullException(nameof(jobConfiguration));
            CancellationToken = cancellationToken;
        }

        public virtual ValueTask DisposeAsync() => ValueTask.CompletedTask;

        public virtual void Dispose()
        {
            GC.SuppressFinalize(true);
        }

        public virtual Task PrepareEnvironmentAsync()
        {
            var initialStep = JobConfiguration.Steps.First();
            initialStep.Cmd = initialStep.Cmd.Replace("[wd]", _workerConfiguration.WorkingDirectory);

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