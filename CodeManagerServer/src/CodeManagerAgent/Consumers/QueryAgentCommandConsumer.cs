using System;
using System.Threading.Tasks;
using CodeManager.Data.Messaging;
using MassTransit;

namespace CodeManagerAgent.Consumers
{
    public class QueryAgentCommandConsumer : IConsumer<QueryAgentCommand>
    {
        /* // TODO: remove this class
         private readonly IAgentService _agentService;
 
         public QueryAgentCommandConsumer(IAgentService agentService)
         {
             _agentService = agentService ?? throw new ArgumentNullException(nameof(agentService));
         }
         
         public Task Consume(ConsumeContext<QueryAgentCommand> context)
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
         }*/
        public Task Consume(ConsumeContext<QueryAgentCommand> context)
        {
            throw new NotImplementedException();
        }
    }
}