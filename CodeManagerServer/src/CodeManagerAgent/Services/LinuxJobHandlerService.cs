using System;
using System.Threading.Tasks;
using CodeManager.Data.Configuration;
using CodeManager.Data.Configuration.StartJob;
using Docker.DotNet;
using MassTransit;
using MassTransit.JobService.Components;
using Microsoft.Extensions.Logging;

namespace CodeManagerAgent.Services
{
    public class LinuxJobHandlerService : JobHandlerService<LinuxJobHandlerService>
    {
        // unit of work
        public LinuxJobHandlerService(string token, JobConfiguration jobConfiguration, Uri responseAddress, ILogger<LinuxJobHandlerService> logger, IBusControl bus, IAgentService agentService)
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

        public async Task StartAsync(JobConfiguration startJobContextConfiguration, Uri responseAddress)
        {
            _jobConfiguration = startJobContextConfiguration;
            _sendEndpoint = await _bus.GetSendEndpoint(responseAddress);
            
            await 
        }
    }
}