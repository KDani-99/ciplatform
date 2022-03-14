using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using CIPlatform.Core.SignalR.Consumers;
using CIPlatform.Data.Agent;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Events;
using CIPlatformWorker.Configuration;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CIPlatformWorker.SignalR
{
    public class WorkerClient : IWorkerClient, IDisposable
    {
        private readonly ILogger<WorkerClient> _logger;

        private readonly IServiceProvider _serviceProvider;

        public WorkerClient(
            IServiceProvider serviceProvider,
                            IOptions<SignalRConfiguration> webSocketConfiguration,
                            IOptions<WorkerConfiguration> agentConfiguration,
                            ILogger<WorkerClient> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var workerConfiguration = agentConfiguration.Value ?? throw new ArgumentNullException(nameof(agentConfiguration));
            var wsConfiguration = webSocketConfiguration.Value ??
                throw new ArgumentNullException(nameof(webSocketConfiguration));

            HubConnection = new HubConnectionBuilder()
                            .WithUrl(
                                $"{wsConfiguration.Host}/{wsConfiguration.Hub}",
                                options =>
                                     {
                                         options.AccessTokenProvider = () => Task.FromResult(string.Empty);
                                         options.Headers.Add("W-JobContext", workerConfiguration.Context.ToString());
                                     })
                            .WithAutomaticReconnect()
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
        
        public HubConnection HubConnection { get; }

        public Task ConfigureAsync()
        {
            return HubConnection.SendAsync("Configure", WorkerState.Available);
        }

        public Task StreamLogAsync(long runId, long jobId, long stepIndex, ChannelReader<string> stream)
        {
            return HubConnection.SendAsync("UploadLogStream", stream, runId, jobId,
                                           stepIndex);
        }

        public Task SendStepResultAsync(StepResultEvent stepResultEvent)
        {
            return HubConnection.SendAsync("StepResultEvent", stepResultEvent);
        }

        public Task PingAsync()
        {
            return HubConnection.SendAsync("Ping");
        }

        public Task FinishJobAsync()
        {
            return HubConnection.SendAsync("FinishJob");
        }

        private void RegisterMethods()
        {
            _logger.LogInformation("Registering worker events...");

            HubConnection.On<QueueJobCommand>(
               "QueueJob",
               async message => await _serviceProvider.GetService<IConsumer<QueueJobCommand>>()!.ConsumeAsync(message));
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
            return ConfigureAsync();
        }
    }
}