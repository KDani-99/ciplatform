using System;
using System.Threading;
using System.Threading.Tasks;
using CodeManager.Core.Hubs.Common;
using CodeManager.Data.Agent;
using CodeManager.Data.Commands;
using CodeManager.Data.Events;
using CodeManagerAgent.Factories;
using CodeManagerAgent.Services;
using MassTransit;
using Microsoft.AspNetCore.SignalR.Client;

namespace CodeManagerAgent.Hubs.Consumers
{
    public abstract class QueueJobEventConsumer
    {
        private readonly IAgentService _agentService;
        private readonly IJobHandlerServiceFactory _jobHandlerServiceFactory;
        private readonly HubConnection _hubConnection;

        protected QueueJobEventConsumer(HubConnection hubConnection ,IAgentService agentService,
            IJobHandlerServiceFactory jobHandlerServiceFactory)
        {
            _hubConnection = hubConnection ?? throw new ArgumentNullException(nameof(hubConnection));
            _agentService = agentService ?? throw new ArgumentNullException(nameof(agentService));
            _jobHandlerServiceFactory = jobHandlerServiceFactory ??
                                        throw new ArgumentNullException(nameof(jobHandlerServiceFactory));
        }

        protected async Task Consume(IQueueJobEvent queueJobEvent)
        {
            if (_agentService.AgentState == AgentState.Available)
            {
                _agentService.AgentState = AgentState.Working;

                var response = await _hubConnection.InvokeAsync<AcceptedRequestJobCommandResponse>(CommonAgentManagerHubMethods.RequestJobCommand, new RequestJobCommand
                {
                    Token = queueJobEvent.Token
                });

                if (response != null)
                {
                    // TODO: only set agent state server side?
                    _agentService.CancellationTokenSource = new CancellationTokenSource();

                    await _jobHandlerServiceFactory.Create(response.Repository,
                            response.Token,
                            response.JobConfiguration,
                            _agentService.CancellationTokenSource.Token)
                        .StartAsync();
                }
                else
                {
                    _agentService.AgentState = AgentState.Available;
                }
            }
            else
            {
                _agentService.AgentState = AgentState.Available;
            }
            // TODO: reset agent state if request times out
            // else busy
        }
    }
}