﻿using System.Linq;
using CodeManager.Data.Entities;
using CodeManagerWebApi.DataTransfer;
using Microsoft.VisualBasic.CompilerServices;

namespace CodeManagerWebApi.Extensions.Entities
{
    public static class ProjectExtensions
    {
        public static ProjectDto ToDto(this Project project)
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

        public static ProjectDataDto ToDataDto(this Project project, Permissions memberPermission)
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