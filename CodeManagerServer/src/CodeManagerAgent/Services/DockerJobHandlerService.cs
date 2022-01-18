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
using CodeManager.Data.Events;
using CodeManagerAgent.Configuration;
using CodeManagerAgent.Exceptions;
using CodeManagerAgentManager.Commands;
using CodeManagerShared.Configuration;
using CodeManagerShared.Events;
using Microsoft.Extensions.Options;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Docker.DotNet;
using Docker.DotNet.Models;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CodeManagerAgent.Services
{
    public sealed class DockerJobHandlerService : JobHandlerService
    {
        private readonly IDockerClient _dockerClient;
        private string _containerId;
        
        // unit of work
        public DockerJobHandlerService(string token, JobConfiguration jobConfiguration, Uri responseAddress, IOptions<AgentConfiguration> agentConfiguration, IDockerClient dockerClient, IBusControl bus, IAgentService agentService)
            : base(token, jobConfiguration, responseAddress, agentConfiguration, bus, agentService)
        {
            _dockerClient = dockerClient ?? throw new ArgumentNullException(nameof(dockerClient));
        }

        /*private Job ParseFileConfiguration()
        {
            // TODO: place it in webapi
            
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(LowerCaseNamingConvention.Instance)
                .Build();
            
            return deserializer.Deserialize<Job>(new StreamReader(_jobConfiguration.ConfigPath));
        }*/

        public override async Task StartAsync()
        {
            var (from, tag) = JobConfiguration.Image.Split(":") switch { var result => (result[0], result[1]) };
            
            await _dockerClient.Images.CreateImageAsync(
                new ImagesCreateParameters
                {
                    FromImage = from,
                    Tag = tag,
                },
                JobConfiguration.AuthConfig,
                new Progress<JSONMessage>());

            var container = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = JobConfiguration.Image,
                //Name = JobConfiguration.AgentId, // TODO: decode jwt and get id
                Env = JobConfiguration.Environment,
            });

            _containerId = container.ID;

            if (!await _dockerClient.Containers.StartContainerAsync(_containerId, new ContainerStartParameters()))
            {
                // TODO: throw exception or send message
                await SendEventAsync(new StepResultEvent
                {
                    State = States.Failed,
                    StepIndex = -1 // start container step
                });
                return;
            }

            await ExecuteJobAsync();
        }

       /* private async Task ExecuteStepsAsync(StepConfiguration step, int index)
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
        }*/

       protected override async Task<long> ExecuteCommandAsync(int stepIndex, string executable, string args, StreamWriter outputStream)
       {
           var execCreateResponse =
               await _dockerClient.Exec.ExecCreateContainerAsync(_containerId, new ContainerExecCreateParameters
               {
                   Cmd = new List<string>() {executable, args},
                   AttachStdout = true,
                   AttachStderr = true
               });
            
            
           var stream = await _dockerClient.Exec.StartAndAttachContainerExecAsync(execCreateResponse.ID, true);
           await ProcessOutputStreamAsync(stepIndex, stream, outputStream);

           return (await _dockerClient.Exec.InspectContainerExecAsync(execCreateResponse.ID)).ExitCode;
       }

       private async Task ProcessOutputStreamAsync(int stepIndex, MultiplexedStream stream, StreamWriter outputStreamWriter)
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
                
                await WriteAndFlushAsync(outputStreamWriter, log.ToString(), stepIndex);
            }
        }

       public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dockerClient?.Containers.StopContainerAsync(_containerId, new ContainerStopParameters()).Wait();
            }
            _dockerClient?.Dispose();
        }

        public override async ValueTask DisposeAsync()
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