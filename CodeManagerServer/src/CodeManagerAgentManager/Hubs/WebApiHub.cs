using System;
using System.Threading.Tasks;
using CodeManager.Data.Commands;
using CodeManagerAgentManager.Repositories;
using CodeManagerAgentManager.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace CodeManagerAgent.Hubs
{
    [Obsolete("Use rabbitmq for webapi-manager communication")]
    public class WebApiHub : Hub
    {
        private readonly IRunService _runService;
        private readonly ILogger<WebApiHub> _logger;
        
        public WebApiHub(IRunService runService, ILogger<WebApiHub> logger)
        {
            _runService = runService ?? throw new ArgumentNullException(nameof(runService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HubMethodName("QueueRunRequest")]
        public async Task<long?> QueueRunRequestAsync(QueueRunCommand request)
        {
            try
            {
                var runId = await _runService.QueueAsync(request);
                return runId;
            }
            catch (Exception exception)
            {
                _logger.LogError($"Failed to consume `{nameof(QueueRunCommand)}`. Error: {exception.Message}");
                return null;
            }
        }
    }
}