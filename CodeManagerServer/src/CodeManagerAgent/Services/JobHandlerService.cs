using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeManager.Data.Configuration;
using CodeManager.Data.Events;
using CodeManager.Data.Extensions;
using CodeManager.Data.JsonWebTokens;
using CodeManagerAgent.Configuration;
using CodeManagerAgent.Exceptions;
using CodeManagerAgent.Extensions;
using Docker.DotNet;
using Docker.DotNet.Models;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;

namespace CodeManagerAgent.Services
{
    public abstract class JobHandlerService : IJobHandlerService
    {
        private readonly AgentConfiguration _agentConfiguration;
        
        protected readonly IBusControl BusControl;
        protected readonly IAgentService AgentService;

        protected readonly JobConfiguration JobConfiguration;
        protected readonly ISendEndpoint SendEndpoint;
        protected readonly string RunId;
        protected readonly string JobId;

        private readonly string _token;

        protected JobHandlerService(string token, JobConfiguration jobConfiguration, Uri responseAddress, IOptions<AgentConfiguration> agentConfiguration, IBusControl busControl, IAgentService agentService)
        {
            // TODO: read config file from mount
            // TODO: or start is remotely as a process and read logs? => could directly stream docker container logs
            _token = token ?? throw new ArgumentNullException(nameof(token));
            _agentConfiguration = agentConfiguration.Value ?? throw new ArgumentNullException(nameof(agentConfiguration));
            BusControl = busControl ?? throw new ArgumentNullException(nameof(busControl));
            AgentService = agentService ?? throw new ArgumentNullException(nameof(agentService));
            JobConfiguration = jobConfiguration ?? throw new ArgumentNullException(nameof(jobConfiguration));
            SendEndpoint = busControl.GetSendEndpoint(responseAddress).Result;

            var decodedToken = _token.DecodeJwtToken();
            RunId = decodedToken.Claims.FirstOrDefault(claim => claim.Type == CustomJwtRegisteredClaimNames.RunId)?.Value;
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
                for (step = 0; step < JobConfiguration.Steps.Count; step++)
                {
                    await SendEventAsync(new StepResultEvent
                    {
                        State = States.Running,
                        StepIndex = step
                    });
                    
                    await ExecuteStepAsync(JobConfiguration.Steps[step], step);
                    
                    await SendEventAsync(new StepResultEvent
                    {
                        State = States.Successful,
                        StepIndex = step
                    });
                }
            }
            catch
            {
                await SendEventAsync(new StepResultEvent
                {
                    State = States.Failed,
                    StepIndex = step
                });
                // rest will be marked as skipped
            }
        }

        protected virtual async Task ExecuteStepAsync(StepConfiguration step, int stepIndex)
        {
            // TODO: log name
            var (executable, args) = step.Cmd.Split(" ") switch { var result =>
                (result[0], string.Join(" ", new ArraySegment<string>(result, 1, result.Length - 1))) };
            
            await using var outputStream = new StreamWriter(File.Open(GetLogFilePath(stepIndex), FileMode.Create, FileAccess.ReadWrite, FileShare.Read));

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
            
            await SendEventAsync(new StepLogEvent
            {
                Log = data,
                StepIndex = stepIndex
            });
        }

        protected Task SendEventAsync<TEvent>(TEvent @event)
        {
            if (@event is ISecureMessage secureMessage)
            {
                secureMessage.Token = _token;
            }
            return SendEndpoint.Send(@event);
        }

        private string GetLogFilePath(int stepIndex)
        {
            return $"{_agentConfiguration.LogDirectory}/{JobId}/step-{stepIndex}.log";
        }
    }
}