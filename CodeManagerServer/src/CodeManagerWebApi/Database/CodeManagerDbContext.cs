using System;
using System.Collections.Generic;
using CodeManager.Data.Entities;
using CodeManagerWebApi.Entities;
using CodeManagerWebApi.Entities.Configuration;
using CodeManagerWebApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace CodeManagerWebApi.Database
{
    public class CodeManagerDbContext : CodeManager.Data.Database.CodeManagerDbContext
    {
        public CodeManagerDbContext(DbContextOptions<CodeManagerDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var config = this.GetService<IConfiguration>();
            var serviceAccountConfiguration =
                config.GetSection("ServiceAccountConfiguration").Get<ServiceAccountConfiguration>();
            var credentialManagerService = this.GetService<ICredentialManagerService>();

            modelBuilder.Entity<Plan>()
                .HasData(new Plan
                {
                    Id = 1,
                    Name = "Basic",
                    MaxCreatedTeamsPerUser = 5,
                    MaxJoinedTeamsPerUser = 15
                });

            if (serviceAccountConfiguration.IsEnabled)
                modelBuilder.Entity<User>()
                    .HasData(new User
                    {
                        Id = 1,
                        IsActive = true,
                        Email = "admin.site@noreply",
                        Username = serviceAccountConfiguration.Username,
                        Password = credentialManagerService.CreateHashedPassword(serviceAccountConfiguration.Password),
                        RegistrationTimestamp = DateTime.Now,
                        Name = "Admin",
                        PlanId = 1,
                        Roles = new [] {Roles.Admin, Roles.User }
                    });
        }
    }
}