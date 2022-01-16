using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CodeManager.Data.Configuration;
using CodeManager.Data.Events;
using CodeManagerAgent.Exceptions;
using Docker.DotNet;
using Docker.DotNet.Models;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CodeManagerAgent.Services
{
    public abstract class JobHandlerService<T> : IJobHandlerService
    {
        private readonly ILogger<T> _logger;
        
        protected readonly IBusControl BusControl;
        protected readonly IAgentService AgentService;

        protected readonly JobConfiguration JobConfiguration;
        protected readonly ISendEndpoint SendEndpoint;

        private readonly string _token;
        
        protected JobHandlerService(string token, JobConfiguration jobConfiguration, Uri responseAddress, ILogger<T> logger, IBusControl busControl, IAgentService agentService)
        {
            // TODO: read config file from mount
            // TODO: or start is remotely as a process and read logs? => could directly stream docker container logs
            _token = token ?? throw new ArgumentNullException(nameof(token));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            BusControl = busControl ?? throw new ArgumentNullException(nameof(busControl));
            AgentService = agentService ?? throw new ArgumentNullException(nameof(agentService));
            JobConfiguration = jobConfiguration ?? throw new ArgumentNullException(nameof(jobConfiguration));
            SendEndpoint = busControl.GetSendEndpoint(responseAddress).Result;
        }
        public abstract ValueTask DisposeAsync();

        public abstract void Dispose();

        public virtual async Task StartAsync()
        {
            // Non zero exit code indicates failure @ the given job
            await ExecuteJobAsync();
        }

        protected virtual async Task ExecuteJobAsync()
        {
            // TODO: signal job start
            var step = -1; // indicated unknown
            try
            {
                for (step = 0; step < _jobConfiguration.Steps.Count; step++)
                {
                    await AgentService.SendAsync(new StepResultEvent
                    {
                        State = States.Running,
                        StepIndex = step
                    }, SendEndpoint);
                    
                    await ExecuteStepAsync(_jobConfiguration.Steps[step], step);
                    
                    await AgentService.SendAsync(new StepResultEvent
                    {
                        State = States.Successful,
                        StepIndex = step
                    }, SendEndpoint);
                }
            }
            catch
            {
                await AgentService.SendAsync(new StepResultEvent
                {
                    State = States.Failed,
                    StepIndex = step
                }, _sendEndpoint);
                // rest will be marked as skipped
            }
        }

        protected virtual async Task ExecuteStepAsync(StepConfiguration step, int stepIndex)
        {
            // TODO: log name
            var (executable, args) = step.Cmd.Split(" ") switch { var result =>
                (result[0], string.Join(" ", new ArraySegment<string>(result, 1, result.Length - 1))) };
            
            await using var outputStream = new StreamWriter(File.Open(Path.Join(_jobConfiguration.LogPath, $"{stepIndex}.log"), FileMode.Create, FileAccess.ReadWrite, FileShare.Read));

            var exitCode = await ExecuteCommandAsync(stepIndex, executable, args, outputStream);
            if (exitCode != 0)
            {
                // LOG exit code
                await WriteAndFlushAsync(outputStream, $"\nExit code: {exitCode}", stepIndex);
                throw new StepFailedException();
            }
        }

        protected abstract Task<long> ExecuteCommandAsync(int stepIndex, string executable, string args,
            StreamWriter outputStream);

        protected async Task WriteAndFlushAsync(StreamWriter outputStreamWriter, string data, int stepIndex)
        {
            await outputStreamWriter.WriteAsync(data);
            await outputStreamWriter.FlushAsync();
            
            await AgentService.SendAsync(new StepLogEvent
            {
                Log = data,
                StepIndex = stepIndex
            }, SendEndpoint);
        }
    }
}