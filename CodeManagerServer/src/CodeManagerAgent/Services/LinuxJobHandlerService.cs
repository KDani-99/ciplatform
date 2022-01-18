using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeManager.Data.Configuration;
using CodeManager.Data.Configuration.StartJob;
using CodeManager.Data.Events;
using CodeManagerAgent.Configuration;
using CodeManagerAgent.Extensions;
using Docker.DotNet;
using MassTransit;
using MassTransit.JobService.Components;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeManagerAgent.Services
{
    public class LinuxJobHandlerService : JobHandlerService
    {
        // unit of work
        public LinuxJobHandlerService(string token, JobConfiguration jobConfiguration, Uri responseAddress, IOptions<AgentConfiguration> agentConfiguration,  IBusControl bus, IAgentService agentService)
            : base(token, jobConfiguration, responseAddress, agentConfiguration, bus, agentService)
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

        protected override async Task<long> ExecuteCommandAsync(int stepIndex, string executable, string args, StreamWriter outputStream)
        {
            using var process = new Process();
            process.ConfigureCliProcess(executable, args, JobConfiguration.Environment); // or /bin/bash -c "<cmd>"

            process.OutputDataReceived += async (_, eventArgs) =>
              await ProcessOnOutputDataReceivedAsync(stepIndex, eventArgs, outputStream);
            process.ErrorDataReceived += async (_, eventArgs) => await ProcessOnErrorDataReceivedAsync(stepIndex, eventArgs, outputStream);

            process.Start();
            
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync();

            return process.ExitCode;
        }

        private async Task ProcessOnErrorDataReceivedAsync(int stepIndex, DataReceivedEventArgs eventArgs, StreamWriter outputStream)
        {
            await WriteAndFlushAsync(outputStream, $"Error: {eventArgs.Data}", stepIndex);
        }

        private async Task ProcessOnOutputDataReceivedAsync(int stepIndex, DataReceivedEventArgs eventArgs, StreamWriter outputStream)
        {
            await WriteAndFlushAsync(outputStream, eventArgs.Data, stepIndex);
        }
    }
}