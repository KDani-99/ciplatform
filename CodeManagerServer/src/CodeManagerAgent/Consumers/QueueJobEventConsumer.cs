using System;
using System.Threading.Tasks;
using CodeManager.Data.Agent;
using CodeManager.Data.Agent.Responses;
using CodeManager.Data.Commands;
using CodeManager.Data.Events;
using CodeManagerAgent.Services;
using MassTransit;

namespace CodeManagerAgent.Consumers
{
    public class QueueJobEventConsumer : IConsumer<QueueJobEvent>
    {
        private readonly IAgentService _agentService;
        private readonly IRequestClient<RequestJobCommand> _requestClient;
        
        public QueueJobEventConsumer(IAgentService agentService)
        {
            _agentService = agentService ?? throw new ArgumentNullException(nameof(agentService));
        }

        public async Task Consume(ConsumeContext<QueueJobEvent> context)
        {
            if (_agentService.AgentState == AgentState.Available)
            {
                _agentService.AgentState = AgentState.Working;

                var requestClient = _requestClient.Create(new RequestJobCommand());
                requestClient.GetResponse<AcceptedRequestJobCommandResponse, RejectedRequestJobCommandResponse>();

                var response = await _requestClient.GetResponse<AcceptedRequestJobCommandResponse, RejectedRequestJobCommandResponse>(new { OrderId = id});

                if (response.Is(out Response<OrderStatusResult> responseA))
                {
                    // do something with the order
                }
                else if (response.Is(out Response<OrderNotFound> responseB))
                {
                    // the order was not found
                }
            }
            // else busy
        }
    }
}