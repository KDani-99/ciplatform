using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using CodeManager.Data.Configuration;
using CodeManagerAgent.Configuration;
using CodeManagerAgent.Entities;
using CodeManagerAgent.Exceptions;
using CodeManagerAgent.Extensions;
using CodeManagerAgent.WebSocket;
using MassTransit;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeManagerAgent.Services
{
    public class LinuxJobHandlerService : JobHandlerService
    {
        // unit of work
        public LinuxJobHandlerService(JobDetails jobDetails, JobConfiguration jobConfiguration, IOptions<AgentConfiguration> agentConfiguration, CancellationToken cancellationToken)
            : base(jobDetails, jobConfiguration, agentConfiguration,
                cancellationToken)
        {
        }

        public override ValueTask DisposeAsync()
        {
            // not required
            return ValueTask.CompletedTask;
        }

        public override void Dispose()
        {
            // not required
        }

        public override async Task ExecuteStepAsync(ChannelWriter<string> channelWriter, StepConfiguration step, int stepIndex)
        {
            await base.ExecuteStepAsync(channelWriter, step, stepIndex);
            
            var command = step.Cmd.Split(" ");
            using var process = new Process();
            process.ConfigureCliProcess(command.Take(1).First(), string.Join(" ", command.Skip(1)),
                JobConfiguration.Environment); // or /bin/bash -c "<cmd>"
            
            process.OutputDataReceived += async (_, eventArgs) =>
                await channelWriter.WriteAsync(eventArgs.Data);
            process.ErrorDataReceived += async (_, eventArgs) => await channelWriter.WriteAsync(eventArgs.Data);

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
                // LOG exit code
                throw new StepFailedException {Name = step.Name};
        }
    }
}