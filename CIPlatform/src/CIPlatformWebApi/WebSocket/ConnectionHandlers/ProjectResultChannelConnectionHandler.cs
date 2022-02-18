using System;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatform.Data.Repositories;
using CIPlatformWebApi.Services;
using CIPlatformWebApi.Services.Project;

namespace CIPlatformWebApi.Strategies
{
    public class ProjectResultChannelConnectionHandler : IResultChannelConnectionHandler
    {
        private readonly IProjectService _projectService;
        
        public ProjectResultChannelConnectionHandler(IProjectService projectService)
        {
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
        }
        
        public Task<bool> VerifyAsync(long entityId, UserEntity user)
        {
            return _projectService.IsAllowedAsync(entityId, user);
        }
    }
}