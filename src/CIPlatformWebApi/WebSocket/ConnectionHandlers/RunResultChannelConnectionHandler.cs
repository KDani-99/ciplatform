using System;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.Services;
using CIPlatformWebApi.Services.Run;

namespace CIPlatformWebApi.Strategies
{
    public class RunResultChannelConnectionHandler : IResultChannelConnectionHandler
    {
        private readonly IRunService _runService;

        public RunResultChannelConnectionHandler(IRunService runService)
        {
            _runService = runService ?? throw new ArgumentNullException(nameof(runService));
        }
        
        public Task<bool> VerifyAsync(long entityId, UserEntity user)
        {
            return _runService.IsAllowedRun(entityId, user);
        }
    }
}