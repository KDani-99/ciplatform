using System.Collections.Generic;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.DataTransfer.Run;

namespace CIPlatformWebApi.DataTransfer.Project
{
    public class ProjectDataDto
    {
        public ProjectDto Project { get; set; }
        public string RepositoryUrl { get; set; }
        public long TeamId { get; set; }
        public Permissions UserPermission { get; set; }
        public IEnumerable<RunDto> Runs { get; set; }
    }
}