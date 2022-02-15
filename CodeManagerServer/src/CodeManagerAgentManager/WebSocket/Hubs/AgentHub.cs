using System;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using CodeManager.Core.Hubs.Clients;
using CodeManager.Data.Agent;
using CodeManager.Data.Configuration;
using CodeManager.Data.Entities;
using CodeManager.Data.Events;
using CodeManagerAgentManager.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace CodeManagerAgentManager.WebSocket.Hubs
{
    public class AgentHub : Hub<IAgentClient>
    {
        private const string HeaderKey = "W-JobContext"; // W for Worker
        private readonly ILogger<AgentHub> _logger;
        private readonly ILogStreamService _logStreamService;
        private readonly IStepService<StepResultEvent> _stepService;
        private readonly IWorkerConnectionService _workerConnectionService;

        public AgentHub(IWorkerConnectionService workerConnectionService,
                        ILogStreamService logStreamService,
                        ILogger<AgentHub> logger,
                        IStepService<StepResultEvent> stepService)
        {
            _workerConnectionService = workerConnectionService ??
                throw new ArgumentNullException(nameof(workerConnectionService));
            _logStreamService = logStreamService ?? throw new ArgumentNullException(nameof(logStreamService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _stepService = stepService ?? throw new ArgumentNullException(nameof(stepService));
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var jobContextString = Context.GetHttpContext().Request.Headers
                                              .First(header => header.Key == HeaderKey).Value;

                if (!Enum.TryParse(jobContextString, out JobContext jobContext))
                    throw new ArgumentException($"Invalid value provided for '{HeaderKey}' header.");

                await _workerConnectionService.AddWorkerConnectionOfTypeAsync(new WorkerConnectionData
                {
                    AgentState = AgentState.Offline, // must be configured first
                    ConnectionId = Context.ConnectionId,
                    JobContext = jobContext
                });
            }
            catch (Exception exception)
            {
                Context.Abort();
                _logger.LogError($"A client tried to connect without a valid '{HeaderKey}' header. Message: " +
                                 exception.Message);
                return;
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _workerConnectionService.RemoveWorkerConnectionAsync(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        [HubMethodName("Configure")]
        public async Task ConfigureWorkerAsync(AgentState agentState)
        {
            try
            {
                await _workerConnectionService.UpdateWorkerConnectionAsync(new WorkerConnectionData
                {
                    AgentState = agentState,
                    ConnectionId = Context.ConnectionId // jobcontext wont be updated
                });
            }
            catch (Exception exception)
            {
                _logger.LogError($"Failed to configure worker. Error: {exception.Message}");
                Context.Abort();
            }
        }

        [HubMethodName("UpdateAgentState")]
        public Task UpdateAgentStateAsync(AgentState agentState)
        {
            return agentState switch
            {
                AgentState.Available => Groups.AddToGroupAsync(Context.ConnectionId, nameof(AgentState.Available)),
                AgentState.Working => Groups.RemoveFromGroupAsync(Context.ConnectionId, nameof(AgentState.Working)),
                _ => throw new ArgumentOutOfRangeException(nameof(agentState))
            };
        }

        [HubMethodName("StepResultEvent")]
        public async Task ReceiveStepResultEventAsync(StepResultEvent stepResultEvent)
        {
            try
            {
                await _stepService.ProcessStepResultAsync(stepResultEvent, Context.ConnectionId);

                // todo: publish rabbitmq ProcessedStepResultEvent that contains the run id, job id and step id
            }
            catch (Exception exception)
            {
                _logger.LogError($"Failed to consume `{nameof(StepResultEvent)}`. Error: {exception.Message}");
            }
        }

        [HubMethodName("UploadLogStream")]
        public Task UploadLogStreamAsync(ChannelReader<string> stream, long runId, long jobId, int step)
        {
            try
            {
                return _logStreamService.ProcessStreamAsync(stream, runId, jobId, step);
            }
            catch (Exception exception)
            {
                _logger.LogError($"An unexpected error has occured. Message: {exception.Message}.");
            }

            return Task.CompletedTask;
        }
    }
}