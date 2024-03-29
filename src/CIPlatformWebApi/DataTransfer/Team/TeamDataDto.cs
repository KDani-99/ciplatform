﻿using System.Collections.Generic;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.DataTransfer.Project;

namespace CIPlatformWebApi.DataTransfer.Team
{
    public class TeamDataDto
    {
        public long Id { get; set; }
        public IEnumerable<TeamMemberDto> Members { get; set; }
        public IEnumerable<ProjectDto> Projects { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public bool IsPublic { get; set; }
        public string Owner { get; set; }
        public Permissions UserPermission { get; set; }
    }
}