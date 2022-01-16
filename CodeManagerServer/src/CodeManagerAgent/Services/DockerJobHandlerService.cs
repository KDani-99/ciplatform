using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using CodeManager.Data.Configuration;
using CodeManager.Data.Configuration.StartJob;
using CodeManager.Data.Entities.CI;
using CodeManagerAgent.Exceptions;
using CodeManagerAgentManager.Commands;
using CodeManagerShared.Configuration;
using CodeManagerShared.Entities;
using CodeManagerShared.Events;
using Microsoft.Extensions.Options;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Docker.DotNet;
using Docker.DotNet.Models;
using MassTransit;
using Microsoft.Extensions.Logging;
using JobConfiguration = CodeManagerAgent.Configuration.JobConfiguration;

namespace CodeManagerAgent.Services
{
    public class JobHandlerService : IJobHandlerService
    {
        private readonly IDockerClient _dockerClient;
        private readonly IPublishEndpoint _publishEndpoint;

        private string _containerId;
        
        // unit of work
        public JobHandlerService(IDockerClient dockerClient, IPublishEndpoint publishEndpoint, IOptions<JobConfiguration> jobConfiguration)
        {
            // TODO: read config file from mount
            // TODO: or start is remotely as a process and read logs? => could directly stream docker container logs
            _dockerClient = dockerClient ?? throw new ArgumentNullException(nameof(dockerClient));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentException(nameof(publishEndpoint));
        }

        private Job ParseFileConfiguration()
        {
            // TODO: place it in webapi
            
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(LowerCaseNamingConvention.Instance)
                .Build();
            
            return deserializer.Deserialize<Job>(new StreamReader(_jobConfiguration.ConfigPath));
        }

        public async Task StartAsync(StartJobConfiguration startJobConfiguration, Uri responseAddress)
        {
            // Non zero exit code indicates failure @ the given job
            var jobConfig = ParseFileConfiguration();

            var (from, tag) = jobConfig.Image.Split(":") switch { var result => (result[0], result[1]) };
            
            await _dockerClient.Images.CreateImageAsync(
                new ImagesCreateParameters
                {
                    FromImage = from,
                    Tag = tag,
                },
                _jobConfiguration.AuthConfig,
                new Progress<JSONMessage>());

            var container = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = jobConfig.Image,
                Name = _jobConfiguration.AgentId,
                Env = jobConfig.Environment,
            });

            _containerId = container.ID;

            if (!await _dockerClient.Containers.StartContainerAsync(_containerId, new ContainerStartParameters()))
            {
                // TODO: throw exception or send message
                await _publishEndpoint.Publish(new StepResultEvent
                {
                    AgentId = _jobConfiguration.AgentId,
                    State = States.Failed,
                    StepIndex = -1 // start container step
                });
                return;
            }

            await ExecuteJobAsync(jobConfig);
        }

        private async Task ExecuteJobAsync(Job job)
        {
            // TODO: signal job start
            var step = -1; // indicated unknown
            try
            {
                for (step = 0; step < job.Steps.Count; step++)
                {
                    await _publishEndpoint.Publish(new StepResultEvent
                    {
                        AgentId = _jobConfiguration.AgentId,
                        State = States.Running,
                        StepIndex = step
                    });
                    
                    await ExecuteStepsAsync(job.Steps[step], step);
                    
                    await _publishEndpoint.Publish(new StepResultEvent
                    {
                        AgentId = _jobConfiguration.AgentId,
                        State = States.Successful,
                        StepIndex = step
                    });
                }
            }
            catch
            {
                await _publishEndpoint.Publish(new StepResultEvent
                {
                    AgentId = _jobConfiguration.AgentId,
                    State = States.Failed,
                    StepIndex = step
                });
                // rest will be marked as skipped
            }
        }

        private async Task ExecuteStepsAsync(Step step, int index)
        {
            // TODO: log name
            var (bin, rest) = step.Cmd.Split(" ") switch { var result =>
                (result[0], string.Join(" ", new ArraySegment<string>(result, 1, result.Length - 1))) };
            var execCreateResponse =
                await _dockerClient.Exec.ExecCreateContainerAsync(_containerId, new ContainerExecCreateParameters
                {
                    Cmd = new List<string>() {bin, rest},
                    AttachStdout = true,
                    AttachStderr = true
                });
            
            
            var stream = await _dockerClient.Exec.StartAndAttachContainerExecAsync(execCreateResponse.ID, true);
            
            await using var outputStream = new StreamWriter(File.Open(Path.Join(_jobConfiguration.LogPath, $"{index}.log"), FileMode.Create, FileAccess.ReadWrite, FileShare.Read));
            await ProcessOutputStreamAsync(stream, outputStream, index);

            var exitCode = (await _dockerClient.Exec.InspectContainerExecAsync(execCreateResponse.ID)).ExitCode;

            if (exitCode != 0)
            {
                // LOG exit code
                await WriteAndFlushAsync(outputStream, $"\nExit code: {exitCode}", index);
                throw new StepFailedException();
            }
        }
        
        /*private async Task ProcessOutputStreamAsync(MultiplexedStream stream, Stream outputStream)
        {
            const int chunkSize = 1024;
            var buffer = new byte[chunkSize];
            
            MultiplexedStream.ReadResult result;
            // TODO: directory will be created by the manager, eg -> /
            // d/ -> /output/artifacts /output/logs
            
            await using var outputStreamWriter = new StreamWriter(outputStream);

            while (!(result = await stream.ReadOutputAsync(buffer, 0, buffer.Length, default)).EOF)
            {
                await outputStreamWriter.WriteAsync(Encoding.UTF8.GetString(new ArraySegment<byte>(buffer, 0, result.Count)));
                await outputStreamWriter.FlushAsync();
            }
        }*/
        private async Task ProcessOutputStreamAsync(MultiplexedStream stream, StreamWriter outputStreamWriter, int index)
        {
            const int chunkSize = 4096; // 4 KB
            var buffer = new byte[chunkSize];
            
            MultiplexedStream.ReadResult result;
            // TODO: directory will be created by the manager, eg -> /
            // d/ -> /output/artifacts /output/logs

            while (!(result = await stream.ReadOutputAsync(buffer, 0, buffer.Length, default)).EOF)
            {
                var log = new StringBuilder(Encoding.UTF8.GetString(new ArraySegment<byte>(buffer, 0, result.Count)));
                if (result.Target == MultiplexedStream.TargetStream.StandardError)
                {
                    log.Insert(0, "Error: ");
                }
                
                await WriteAndFlushAsync(outputStreamWriter, log.ToString(), index);
                // TODO: connect to socketio server from here and send log directly?
            }
        }

        private async Task WriteAndFlushAsync(StreamWriter outputStreamWriter, string data, int index)
        {
            await outputStreamWriter.WriteAsync(data);
            await outputStreamWriter.FlushAsync();
            
            await _publishEndpoint.Publish(new StepLogEvent
            {
                Message = data,
                AgentId = _jobConfiguration.AgentId,
                StepIndex = index
            });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dockerClient?.Containers.StopContainerAsync(_containerId, new ContainerStopParameters()).Wait();
            }
            _dockerClient?.Dispose();
        }

        public virtual async ValueTask DisposeAsync()
        {
            if (_dockerClient is not null)
            {
                await _dockerClient.Containers.StopContainerAsync(_containerId, new ContainerStopParameters()).ConfigureAwait(false);
            }
            Dispose(false);
            GC.SuppressFinalize(this);
        }
    }
}