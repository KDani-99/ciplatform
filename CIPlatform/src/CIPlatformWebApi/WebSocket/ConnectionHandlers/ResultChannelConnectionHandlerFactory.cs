using System;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.Services;

namespace CIPlatformWebApi.Strategies
{
    public class ResultChannelConnectionHandlerFactory : IResultChannelConnectionHandlerFactory
    {
        private readonly IProjectService _projectService;
        private readonly IRunService _runService;
        
        public ResultChannelConnectionHandlerFactory(IProjectService projectService, IRunService runService)
        {
            _projectService = projectService ?? throw new ArgumentException(nameof(projectService));
            _runService = runService ?? throw new ArgumentNullException(nameof(runService));
        }

        public IResultChannelConnectionHandler Create(string type)
        {
            return type switch
            {
                "project" => new ProjectResultChannelConnectionHandler(_projectService),
                "run" => new RunResultChannelConnectionHandler(_runService),
                "job" => new JobResultChannelConnectionHandler(_runService),
                "step" => new StepResultChannelConnectionHandler(_runService),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}