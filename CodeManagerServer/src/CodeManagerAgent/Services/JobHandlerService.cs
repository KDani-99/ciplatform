using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeManagerAgent.Configuration;
using CodeManagerAgent.Exceptions;
using CodeManagerShared.Configuration;
using CodeManagerShared.Entities;
using CodeManagerShared.Events;
using Microsoft.Extensions.Options;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Docker.DotNet;
using Docker.DotNet.Models;
using MassTransit;

namespace CodeManagerAgent.Services
{
    public class JobHandlerService : IJobHandlerService
    {
        private readonly JobConfiguration _jobConfiguration;
        private readonly IDockerClient _dockerClient;
        private readonly IPublishEndpoint _publishEndpoint; 

        private string containerId;
        
        // unit of work
        public JobHandlerService(IOptions<JobConfiguration> jobConfiguration, IDockerClient dockerClient)
        {
            // TODO: read config file from mount
            // TODO: or start is remotely as a process and read logs? => could directly stream docker container logs
            _jobConfiguration = jobConfiguration.Value ?? throw new ArgumentNullException(nameof(jobConfiguration));
            _dockerClient = dockerClient ?? throw new ArgumentNullException(nameof(dockerClient));
        }

        private Job ParseFileConfiguration()
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(LowerCaseNamingConvention.Instance)
                .Build();
            
            return deserializer.Deserialize<Job>(new StreamReader(_jobConfiguration.ConfigPath));
        }

        public async Task StartAsync()
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

            containerId = container.ID;

            if (!await _dockerClient.Containers.StartContainerAsync(containerId, new ContainerStartParameters()))
            {
                // TODO: throw exception or send message
            }
        }

        private async Task ExecuteJobAsync(Job job)
        {
            // TODO: signal job start
            int step = -1; // indicated unknown
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
                        State = States.Success,
                        StepIndex = step
                    });
                }
            }
            catch
            {
                await _publishEndpoint.Publish(new StepResultEvent
                {
                    AgentId = _jobConfiguration.AgentId,
                    State = States.Fail,
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
                await _dockerClient.Exec.ExecCreateContainerAsync(containerId, new ContainerExecCreateParameters
                {
                    Cmd = new List<string>() {bin, rest},
                    AttachStdout = true,
                    AttachStderr = true
                });
            
            
            var stream = await _dockerClient.Exec.StartAndAttachContainerExecAsync(execCreateResponse.ID, true);
            
            await using var outputStream = File.Open(Path.Join(_jobConfiguration.LogPath, $"{index}.log"), FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            await ProcessOutputStreamAsync(stream, outputStream, index);

            var exitCode = (await _dockerClient.Exec.InspectContainerExecAsync(execCreateResponse.ID)).ExitCode;

            if (exitCode != 0)
            {
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
        private async Task ProcessOutputStreamAsync(MultiplexedStream stream, Stream outputStream, int index)
        {
            const int chunkSize = 4096; // 4 KB
            var buffer = new byte[chunkSize];
            
            MultiplexedStream.ReadResult result;
            // TODO: directory will be created by the manager, eg -> /
            // d/ -> /output/artifacts /output/logs
            
            await using var outputStreamWriter = new StreamWriter(outputStream);

            while (!(result = await stream.ReadOutputAsync(buffer, 0, buffer.Length, default)).EOF)
            {
                if (result.Target == MultiplexedStream.TargetStream.StandardError)
                {
                    await WriteAndFlushAsync(outputStreamWriter, "Error: ");
                }

                var log = Encoding.UTF8.GetString(new ArraySegment<byte>(buffer, 0, result.Count));
                
                await WriteAndFlushAsync(outputStreamWriter, log);
                // TODO: connect to socketio server from here and send log directly?
                await _publishEndpoint.Publish(new StepLogEvent
                {
                    Message = log,
                    AgentId = _jobConfiguration.AgentId,
                    StepIndex = index
                });
            }
        }

        private async Task WriteAndFlushAsync(StreamWriter outputStreamWriter, string data)
        {
            await outputStreamWriter.WriteAsync(data);
            await outputStreamWriter.FlushAsync();
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
                _dockerClient?.Containers.StopContainerAsync(containerId, new ContainerStopParameters()).Wait();
            }
            _dockerClient?.Dispose();
        }

        public virtual async ValueTask DisposeAsync()
        {
            if (_dockerClient is not null)
            {
                await _dockerClient.Containers.StopContainerAsync(containerId, new ContainerStopParameters()).ConfigureAwait(false);
            }
            Dispose(false);
            GC.SuppressFinalize(this);
        }
    }
}