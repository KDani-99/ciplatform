using System;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.Services;

namespace CIPlatformWebApi.Strategies
{
    public class StepResultChannelConnectionHandler : IResultChannelConnectionHandler
    {
        private readonly IRunService _runService;
        public StepResultChannelConnectionHandler(IRunService runService)
        {
            _runService = runService ?? throw new ArgumentNullException(nameof(runService));
        }
        public Task<bool> VerifyAsync(long entityId, User user)
        {
            return _runService.IsAllowedStep(entityId, user);
        }
    }
}