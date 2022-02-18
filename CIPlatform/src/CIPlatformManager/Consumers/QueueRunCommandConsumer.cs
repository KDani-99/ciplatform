using System;
using System.Threading.Tasks;
using CIPlatform.Data.Commands;
using CIPlatformManager.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CIPlatformManager.Consumers
{
    public class QueueRunCommandConsumer : IConsumer<QueueRunCommand>
    {
        private readonly ILogger<QueueRunCommandConsumer> _logger;
        private readonly IRunService _runService;

        public QueueRunCommandConsumer(IRunService runService, ILogger<QueueRunCommandConsumer> logger)
        {
            _runService = runService ?? throw new ArgumentNullException(nameof(runService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<QueueRunCommand> context)
        {
            try
            {
                await _runService.QueueAsync(context.Message);
                _logger.LogInformation($"Queued run with id `{context.Message.RunId}`.");
            }
            catch (Exception exception)
            {
                _logger.LogError($"Failed to consume `{nameof(QueueRunCommand)}`. Error: {exception.Message}");
                await context.RespondAsync(new FailedQueueRunCommandResponse());
            }
        }
    }
}