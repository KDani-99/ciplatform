using System;
using System.Threading.Tasks;
using CodeManager.Data.Agent;
using CodeManager.Data.Agent.Requests;
using CodeManager.Data.Agent.Responses;
using CodeManagerAgent.Services;
using MassTransit;

namespace CodeManagerAgent.Consumers
{
    public class AgentRequestRequestConsumer : IConsumer<AgentRequestRequest>
    {
        private readonly IAgentService _agentService;
        private readonly IBusControl _busControl;
        
        public AgentRequestRequestConsumer(IAgentService agentService, IBusControl busControl)
        {
            _agentService = agentService ?? throw new ArgumentNullException(nameof(agentService));
            _busControl = busControl ?? throw new ArgumentNullException(nameof(busControl));
        }
        public async Task Consume(ConsumeContext<AgentRequestRequest> context)
        {
            if (_agentService.AgentState == AgentState.Available)
            {
                _agentService.AgentState = AgentState.Working;
                await context.RespondAsync(new AgentRequestResponse
                {
                    AgentState = AgentState.Available
                });
            }
            else
            {
                // if not available, then send the message back to the queue
                var endpoint = await _busControl.GetSendEndpoint(context.DestinationAddress);
                await endpoint.Send(context.Message);
            }
        }
    }
}