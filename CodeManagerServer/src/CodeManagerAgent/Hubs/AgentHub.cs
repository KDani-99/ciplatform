using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeManager.Data.Events;
using CodeManagerAgent.Consumers;
using CodeManagerAgent.Hubs.Consumers;
using GreenPipes.Agents;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using QueueDockerJobEventConsumer = CodeManagerAgent.Hubs.Consumers.QueueDockerJobEventConsumer;

namespace CodeManagerAgent.Hubs
{
    public class AgentHub
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly HubConnection _hubConnection;
        
        public AgentHub(IServiceProvider serviceProvider, HubConnection hubConnection)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _hubConnection = hubConnection ?? throw new ArgumentNullException(nameof(hubConnection));

            RegisterMethods();
        }

        private void RegisterMethods()
        {
            _hubConnection.On<QueueDockerJobEvent>(CommonHubMethods.QueueDockerJobEvent,  (message) => _serviceProvider.GetService<IConsumer<QueueDockerJobEvent>>()?.Consume(message));
            _hubConnection.On<QueueLinuxJobEvent>(CommonHubMethods.QueueLinuxJobEvent,  (message) => _serviceProvider.GetService<IConsumer<QueueLinuxJobEvent>>()?.Consume(message));
            _hubConnection.On<QueueWindowsJobEvent>(CommonHubMethods.QueueWindowsJobEvent,  (message) => _serviceProvider.GetService<IConsumer<QueueWindowsJobEvent>>()?.Consume(message));
        }
    }
}