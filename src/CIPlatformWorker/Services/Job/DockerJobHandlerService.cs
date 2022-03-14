using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Events;
using CIPlatformWorker.Configuration;
using CIPlatformWorker.Entities;
using CIPlatformWorker.Exceptions;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Options;

namespace CIPlatformWorker.Services.Job
{
    public class DockerJobHandlerService : JobHandlerService
    {
        private readonly IDockerClient _dockerClient;
        private string _containerId;

        // unit of work
        public DockerJobHandlerService(
                                       JobConfiguration jobConfiguration,
                                       IOptions<WorkerConfiguration> agentConfiguration,
                                       IDockerClient dockerClient,
                                       CancellationToken cancellationToken) :
            base(jobConfiguration, agentConfiguration,
                 cancellationToken)
        {
            _dockerClient = dockerClient ?? throw new ArgumentNullException(nameof(dockerClient));
        }

        public override async Task PrepareEnvironmentAsync()
        {
            await base.PrepareEnvironmentAsync();

            var (from, tag) = JobConfiguration.Image.Split(":") switch { var result => (result[0], result[1]) };

            await _dockerClient.Images.CreateImageAsync(
                new ImagesCreateParameters
                {
                    FromImage = from,
                    Tag = tag
                },
                JobConfiguration.AuthConfig,
                new Progress<JSONMessage>());

            var container = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = JobConfiguration.Image,
                Env = JobConfiguration.Environment,
                AttachStdout = true,
                AttachStderr = true,
                Tty = true // keeps detached container running
            });

            _containerId = container.ID;

            if (!await _dockerClient.Containers.StartContainerAsync(_containerId, new ContainerStartParameters()))
            {
                throw new StepFailedException(
                    $"Failed to start {nameof(JobContext.Docker)} container. Container id: {_containerId}");
            }
        }

        public override async Task ExecuteStepAsync(
            ChannelWriter<string> channelWriter,
                                                    StepConfiguration step,
                                                    int stepIndex)
        {
            await base.ExecuteStepAsync(channelWriter, step, stepIndex);

            var command = step.Cmd.Split(" ");

            var execCreateResponse =
                await _dockerClient.Exec.ExecCreateContainerAsync(_containerId, new ContainerExecCreateParameters
                {
                    Cmd = command,
                    AttachStdout = true,
                    AttachStderr = true
                });


            var stream =
                await _dockerClient.Exec.StartAndAttachContainerExecAsync(execCreateResponse.ID, false); // true?

            await ProcessOutputStreamAsync(channelWriter, stream);

            var exitCode = (await _dockerClient.Exec.InspectContainerExecAsync(execCreateResponse.ID)).ExitCode;
            await channelWriter.WriteAsync($"Exit code: {exitCode}{Environment.NewLine}");
            if (exitCode != 0)
            {
                throw new StepFailedException { Name = step.Name, ExitCode = exitCode };
            }
        }

        private async Task ProcessOutputStreamAsync(ChannelWriter<string> channelWriter, MultiplexedStream stream)
        {
            const int chunkSize = 4096; // 4 KiB
            var buffer = new byte[chunkSize];

            MultiplexedStream.ReadResult result;

            var log = new StringBuilder();
            while (!(result = await stream.ReadOutputAsync(buffer, 0, buffer.Length, default)).EOF)
            {
                log.Append(Encoding.UTF8.GetString(new ArraySegment<byte>(buffer, 0, result.Count)));

                await StreamLinesAsync(log, channelWriter);

                CancellationToken.ThrowIfCancellationRequested();
            }

            await StreamLinesAsync(log, channelWriter);

            // in case there is something left in the string builder
            // stream rest of the line
            var dataToStream = log.ToString();
            await channelWriter.WriteAsync(dataToStream);
        }

        private async Task StreamLinesAsync(StringBuilder multiLineLog, ChannelWriter<string> channelWriter)
        {
            await foreach (var line in ProcessStringBuilder(multiLineLog))
            {
                await channelWriter.WriteAsync(line);
            }
        }

        private static async IAsyncEnumerable<string> ProcessStringBuilder(StringBuilder sb)
        {
            for (var i = 0; i < sb.Length; i++)
            {
                // stream lines only
                if (sb[i] != '\n')
                {
                    continue;
                }

                await Task.Yield();
                yield return sb.ToString(0, i + 1);
                sb.Remove(0, i + 1);
                i -= i + 1;
            }
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && _containerId != null)
            {
                _dockerClient?.Containers.StopContainerAsync(_containerId, new ContainerStopParameters()).Wait();
            }

            _dockerClient?.Dispose();
        }

        public override async ValueTask DisposeAsync()
        {
            if (_dockerClient?.Containers != null && _containerId != null)
            {
                await _dockerClient.Containers.StopContainerAsync(_containerId, new ContainerStopParameters())
                                   .ConfigureAwait(false);
                await _dockerClient.Containers.RemoveContainerAsync(_containerId, new ContainerRemoveParameters()).ConfigureAwait(false);
            }

            Dispose(false);
            GC.SuppressFinalize(this);
        }
    }
}