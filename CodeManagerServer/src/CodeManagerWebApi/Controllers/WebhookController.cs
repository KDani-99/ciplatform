using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.WebHooks;
namespace CodeManagerWebApi.Controllers
{
    [ApiController]
    [Route("/api/webhooks")]
    public class WebhookController : ControllerBase, IWebHookReceiver
    {
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(ILogger<WebhookController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [GitHubWebHook]
        public Task<IActionResult> ReceiveWebhook()
        {
            return Task.FromResult(null as IActionResult);
        }

        public bool IsApplicable(string receiverName)
        {
            throw new NotImplementedException();
        }

        public string ReceiverName { get; }
    }
}