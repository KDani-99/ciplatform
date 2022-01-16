using System;
using System.Threading.Tasks;
using CodeManager.Data.Configuration;
using CodeManager.Data.Configuration.StartJob;
using Docker.DotNet;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CodeManagerAgent.Services
{
    public class WindowsJobHandlerService : JobHandlerService<WindowsJobHandlerService>
    {
        // unit of work
        public WindowsJobHandlerService(string token, JobConfiguration jobConfiguration, Uri responseAddress, ILogger<WindowsJobHandlerService> logger, IBusControl bus, IAgentService agentService)
            : base(token, jobConfiguration, responseAddress, logger, bus, agentService)
        {
        }
        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task StartAsync(StartJobContextConfiguration startJobContextConfiguration, Uri responseAddress)
        {
            _jobConfiguration = startJobContextConfiguration;
            _sendEndpoint = await _bus.GetSendEndpoint(responseAddress);
        }
    }
}