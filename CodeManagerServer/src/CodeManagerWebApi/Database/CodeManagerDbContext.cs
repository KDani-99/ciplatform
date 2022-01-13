using System;
using System.Collections.Generic;
using CodeManagerWebApi.Entities;
using CodeManagerWebApi.Entities.Configuration;
using CodeManagerWebApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace CodeManagerWebApi.Database
{
    public class CodeManagerDbContext : DbContext
    {
        public CodeManagerDbContext(DbContextOptions<CodeManagerDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        static CodeManagerDbContext()
        {
            NpgsqlConnection.GlobalTypeMapper.MapEnum<Roles>();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<LoginHistory> LoginHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<Roles>();
            
            var config = this.GetService<IConfiguration>();
            var serviceAccountConfiguration =
                config.GetSection("ServiceAccountConfiguration").Get<ServiceAccountConfiguration>();
            var credentialManagerService = this.GetService<ICredentialManagerService>();

            modelBuilder.Entity<User>()
                .HasOne(user => user.Plan);

            modelBuilder.Entity<TeamMember>()
                .HasOne(teamMember => teamMember.Team)
                .WithMany(team => team.Members);

            modelBuilder.Entity<LoginHistory>()
                .HasOne(history => history.User)
                .WithMany(user => user.LoginHistory);

            modelBuilder.Entity<Team>()
                .HasMany(team => team.Projects)
                .WithOne(project => project.Team);

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