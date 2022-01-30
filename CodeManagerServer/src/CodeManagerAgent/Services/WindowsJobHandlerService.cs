using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using CodeManager.Data.Configuration;
using CodeManagerAgent.Configuration;
using CodeManagerAgent.Exceptions;
using CodeManagerAgent.Extensions;
using CodeManagerAgent.WebSocket;
using MassTransit;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeManagerAgent.Services
{
    public class WindowsJobHandlerService : JobHandlerService
    {
        // unit of work
        public WindowsJobHandlerService(string repository, string token, JobConfiguration jobConfiguration,
            IWorkerClient workerClient, IOptions<AgentConfiguration> agentConfiguration, ILogger<JobHandlerService> logger, CancellationToken cancellationToken)
            : base(repository, token, jobConfiguration, workerClient, agentConfiguration, logger,
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

        protected override async Task ExecuteStepAsync(StepConfiguration step, int stepIndex)
        {
            var command = step.Cmd.Split(" ");
            using var process = new Process();
            process.ConfigureCliProcess(command.Take(1).First(), string.Join(" ", command.Skip(1)),
                JobConfiguration.Environment); // or /bin/bash -c "<cmd>"

            var channel = Channel.CreateUnbounded<string>();

            process.OutputDataReceived += async (_, eventArgs) =>
                await channel.Writer.WriteAsync(eventArgs.Data);
            process.ErrorDataReceived += async (_, eventArgs) => await channel.Writer.WriteAsync(eventArgs.Data);

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync();
            channel.Writer.Complete();

            if (process.ExitCode != 0)
                // LOG exit code
                throw new StepFailedException {Name = step.Name};
        }
    }
}