﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeManager.Data.Configuration;
using CodeManager.Data.Configuration.StartJob;
using CodeManagerAgent.Configuration;
using CodeManagerAgent.Extensions;
using Docker.DotNet;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeManagerAgent.Services
{
    public class WindowsJobHandlerService : JobHandlerService
    {
        // unit of work
        public WindowsJobHandlerService(string repository, string token, JobConfiguration jobConfiguration, IOptions<AgentConfiguration> agentConfiguration, IBusControl bus, IAgentService agentService, CancellationToken cancellationToken)
            : base(repository, token, jobConfiguration, agentConfiguration, bus, agentService, cancellationToken)
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

        protected override async Task<long> ExecuteCommandAsync(int stepIndex, IList<string> command, StreamWriter outputStream)
        {
            using var process = new Process();
            process.ConfigureCliProcess(command.Take(1).First(), string.Join(" ", command.Skip(1)),JobConfiguration.Environment); // or /bin/bash -c "<cmd>"


            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.OutputDataReceived += async (_, eventArgs) =>
                await ProcessOnOutputDataReceivedAsync(stepIndex, eventArgs, outputStream);
            process.ErrorDataReceived += async (_, eventArgs) => await ProcessOnErrorDataReceivedAsync(stepIndex, eventArgs, outputStream);

            process.Start();

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