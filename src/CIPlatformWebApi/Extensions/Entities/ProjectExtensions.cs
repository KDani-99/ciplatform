﻿using System.Linq;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.DataTransfer;
using CIPlatformWebApi.DataTransfer.Project;

namespace CIPlatformWebApi.Extensions.Entities
{
    public static class ProjectExtensions
    {
        public static ProjectDto ToDto(this ProjectEntity project)
        {
            return new()
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                IsPrivateProject = project.IsPrivateProject,
                TeamName = project.Team.Name,
                Owner = project.Team.Name,
                Runs = project.Runs.Count
            };
        }

        public static ProjectDataDto ToDataDto(this ProjectEntity project, Permissions memberPermission)
        {
            return new()
            {
                Project = project.ToDto(),
                Runs = project.Runs.Select(run => run.ToDto()),
                TeamId = project.Team.Id,
                UserPermission = memberPermission,
                RepositoryUrl = project.RepositoryUrl,
            };
        }
    }
}