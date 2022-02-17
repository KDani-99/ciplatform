using System;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.Services;

namespace CIPlatformWebApi.Strategies
{
    public class RunResultChannelConnectionHandler : IResultChannelConnectionHandler
    {
        private readonly IRunService _runService;

        public RunResultChannelConnectionHandler(IRunService runService)
        {
            _runService = runService ?? throw new ArgumentNullException(nameof(runService));
        }
        
        public Task<bool> VerifyAsync(long entityId, User user)
        {
            return _runService.IsAllowedRun(entityId, user);
        }
    }
}