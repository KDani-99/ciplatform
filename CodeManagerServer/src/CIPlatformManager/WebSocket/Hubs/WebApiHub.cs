using System;
using System.Threading.Tasks;
using CIPlatform.Data.Commands;
using CIPlatformManager.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace CIPlatformWorker.Hubs
{
    public class WebApiHub : Hub
    {
        private readonly ILogger<WebApiHub> _logger;
        private readonly IRunService _runService;

        public WebApiHub(IRunService runService, ILogger<WebApiHub> logger)
        {
            _runService = runService ?? throw new ArgumentNullException(nameof(runService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HubMethodName("QueueRun")]
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