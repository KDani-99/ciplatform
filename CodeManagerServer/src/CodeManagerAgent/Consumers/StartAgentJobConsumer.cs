using System;
using System.Threading.Tasks;
using CodeManager.Data.Messaging;
using CodeManagerAgent.Factories;
using CodeManagerAgent.Services;
using MassTransit.JobService;

namespace CodeManagerAgent.Consumers
{
    public class StartAgentJobConsumer : IJobConsumer<StartAgentJob>
    {
        private readonly IJobHandlerServiceFactory _jobHandlerServiceFactory; // TODO: receive factory

        public StartAgentJobConsumer(IJobHandlerServiceFactory jobHandlerServiceFactory)
        {
            _jobHandlerServiceFactory = jobHandlerServiceFactory ?? throw new ArgumentNullException(nameof(jobHandlerServiceFactory));
        }
        
        public async Task Run(JobContext<StartAgentJob> context)
        {
            var jobHandlerService = _jobHandlerServiceFactory.Create(context.Job.Token, context.Job.JobConfiguration, context.ResponseAddress);
            await jobHandlerService.StartAsync(); // unit of work
        }
    }
}