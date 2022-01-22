using System;
using System.Threading;
using System.Threading.Tasks;
using CodeManager.Data.Agent;
using CodeManager.Data.Agent.Responses;
using CodeManager.Data.Commands;
using CodeManager.Data.Events;
using CodeManagerAgent.Factories;
using CodeManagerAgent.Services;
using MassTransit;

namespace CodeManagerAgent.Consumers
{
    public abstract class QueueJobEventConsumer : IConsumer<IQueueJobEvent>
    {
        private readonly IAgentService _agentService;
        private readonly IRequestClient<RequestJobCommand> _requestClient;
        private readonly IJobHandlerServiceFactory _jobHandlerServiceFactory;
        
        protected QueueJobEventConsumer(IAgentService agentService, IRequestClient<RequestJobCommand> requestClient, IJobHandlerServiceFactory jobHandlerServiceFactory)
        {
            _agentService = agentService ?? throw new ArgumentNullException(nameof(agentService));
            _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
            _jobHandlerServiceFactory = jobHandlerServiceFactory ??
                                        throw new ArgumentNullException(nameof(jobHandlerServiceFactory));
        }

        public virtual async Task Consume(ConsumeContext<IQueueJobEvent> context)
        {
            if (_agentService.AgentState == AgentState.Available)
            {
                _agentService.AgentState = AgentState.Working;

                var response = await _requestClient.GetResponse<AcceptedRequestJobCommandResponse, RejectedRequestJobCommandResponse>(new RequestJobCommand
                {
                    Token = context.Message.Token
                });

                if (response.Is(out Response<AcceptedRequestJobCommandResponse> acceptedRequestJobCommandResponse))
                {
                    // start working
                    _agentService.CancellationTokenSource = new CancellationTokenSource();
                    
                    await _jobHandlerServiceFactory.Create(acceptedRequestJobCommandResponse.Message.Token, acceptedRequestJobCommandResponse.Message.JobConfiguration, _agentService.CancellationTokenSource.Token)
                        .StartAsync();
                }
                else if (response.Is(out Response<RejectedRequestJobCommandResponse> rejectedRequestJobCommandResponse))
                {
                    _agentService.AgentState = AgentState.Available;
                }
            }
            // else busy
        }

        public Task Consume(ConsumeContext<QueueLinuxJobEvent> context)
        {
            throw new NotImplementedException();
        }

        public Task Consume(ConsumeContext<QueueWindowsJobEvent> context)
        {
            throw new NotImplementedException();
        }
    }
}