using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using CodeManager.Core.Hubs.Common;
using CodeManager.Core.Hubs.Consumers;
using CodeManager.Data.Agent;
using CodeManager.Data.Configuration;
using CodeManager.Data.Entities;
using CodeManager.Data.Events;
using CodeManagerAgent.Configuration;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeManagerAgent.WebSocket
{
    public class WorkerClient : IWorkerClient, IDisposable
    {
        private readonly AgentConfiguration _agentConfiguration;
        private readonly ILogger<WorkerClient> _logger;

        private readonly IServiceProvider _serviceProvider;

        public WorkerClient(IServiceProvider serviceProvider,
                            IOptions<WebSocketConfiguration> webSocketConfiguration,
                            IOptions<AgentConfiguration> agentConfiguration,
                            ILogger<WorkerClient> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _agentConfiguration =
                agentConfiguration.Value ?? throw new ArgumentNullException(nameof(agentConfiguration));
            var wsConfiguration = webSocketConfiguration.Value ??
                throw new ArgumentNullException(nameof(webSocketConfiguration));

            HubConnection = new HubConnectionBuilder()
                            .WithUrl($"{wsConfiguration.Host}/{wsConfiguration.Hub}",
                                     options =>
                                     {
                                         options.AccessTokenProvider = () => Task.FromResult("");
                                         options.Headers.Add("W-JobContext", _agentConfiguration.Context.ToString());
                                     })
                            .Build();

            HubConnection.Reconnecting += OnReconnection;
            HubConnection.Reconnected += OnReconnected;
            HubConnection.Closed += OnConnectionClose;

            HubConnection.ServerTimeout = TimeSpan.FromSeconds(180);
            HubConnection.HandshakeTimeout = TimeSpan.FromSeconds(60);

            RegisterMethods();
        }

        public void Dispose()
        {
            HubConnection.Reconnected -= OnReconnected;
            HubConnection.Reconnecting -= OnReconnection;
            HubConnection.Closed -= OnConnectionClose;
            HubConnection?.DisposeAsync();
        }

        // TODO: do the same with this class as I did with redis, register this class as singleton
        public HubConnection HubConnection { get; }

        public Task ConfigureAsync()
        {
            return HubConnection.SendAsync("Configure", new HostMachineInformation(), AgentState.Available);
        }

        public Task StreamLogAsync(long runId, long jobId, long stepIndex, ChannelReader<string> stream)
        {
            return HubConnection.SendAsync("UploadLogStream", stream, runId, jobId,
                                           stepIndex); // TODO: CommonHubMethods const in Attribute
        }

        public Task SendStepResult(StepResultEvent stepResultEvent)
        {
            return HubConnection.SendAsync(CommonAgentManagerHubMethods.StepResultEvent, stepResultEvent);
        }

        private void RegisterMethods()
        {
            _logger.LogInformation("Registering worker events...");

            var methodName = _agentConfiguration.Context switch
            {
                JobContext.Docker => CommonHubMethods.QueueDockerJob,
                JobContext.Linux => CommonHubMethods.QueueLinuxJob,
                JobContext.Windows => CommonHubMethods.QueueWindowsJob
            };

            HubConnection.On<QueueJobCommand>(methodName,
                                              message =>
                                              {
                                                  try
                                                  {
                                                      var x = _serviceProvider.GetService<IConsumer<QueueJobCommand>>();
                                                      Console.WriteLine(x.GetType());
                                                      x.ConsumeAsync(message);
                                                  }
                                                  catch (Exception ex)
                                                  {
                                                      Console.WriteLine(ex);
                                                  }
                                              });

            /*  HubConnection.On<QueueJobEvent>(CommonHubMethods.QueueDockerJobEvent,
                  message => _serviceProvider.GetService<IConsumer<QueueDockerJobEvent>>()?.Consume(message));
              HubConnection.On<QueueJobEvent>(CommonHubMethods.QueueLinuxJobEvent,
                  message => _serviceProvider.GetService<IConsumer<QueueLinuxJobEvent>>()?.Consume(message));
              HubConnection.On<QueueJobEvent>(CommonHubMethods.QueueWindowsJobEvent,
                  message => _serviceProvider.GetService<IConsumer<QueueWindowsJobEvent>>()?.Consume(message));*/
        }


        private Task OnConnectionClose(Exception exception)
        {
            _logger.LogError("Connection lost. Error: " + exception.Message);
            return Task.CompletedTask;
        }

        private Task OnReconnection(Exception _)
        {
            _logger.LogWarning("Reconnecting...");
            return Task.CompletedTask;
        }

        private Task OnReconnected(string _)
        {
            _logger.LogError("Reconnected to remote host.");
            return Task.CompletedTask;
        }
    }
}