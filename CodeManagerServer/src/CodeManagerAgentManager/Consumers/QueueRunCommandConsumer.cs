using System;
using System.Threading.Tasks;
using CodeManager.Data.Commands;
using CodeManagerAgentManager.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CodeManagerAgentManager.Consumers
{
    public class QueueRunCommandConsumer : IConsumer<QueueRunCommand>
    {
        private readonly IRunService<QueueRunCommand> _runService;
        private readonly ILogger<QueueRunCommandConsumer> _logger;

        public QueueRunCommandConsumer(IRunService<QueueRunCommand> runService, ILogger<QueueRunCommandConsumer> logger)
        {
            _runService = runService ?? throw new ArgumentNullException(nameof(runService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task Consume(ConsumeContext<QueueRunCommand> context)
        {
            try
            {
                var runId = await _runService.QueueAsync(context.Message);

                await context.RespondAsync(new QueueRunCommandResponse
                {
                    RunId = runId
                });
            }
            catch (Exception exception)
            {
                // TODO: send error
                _logger.LogError($"Failed to consume `{nameof(QueueRunCommand)}`. Error: {exception.StackTrace}");
            }
        }
    }
}