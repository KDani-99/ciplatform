using System.Collections;
using System.Collections.Generic;
using CodeManager.Data.Entities;

namespace CodeManagerWebApi.DataTransfer
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