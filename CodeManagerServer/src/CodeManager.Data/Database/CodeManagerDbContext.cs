﻿using CodeManager.Data.Configuration;
using CodeManager.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Npgsql;

using Run = CodeManager.Data.Entities.CI.Run;
using Job = CodeManager.Data.Entities.CI.Job;
using Step = CodeManager.Data.Entities.CI.Step;

namespace CodeManager.Data.Database
{
    public class CodeManagerDbContext : DbContext
    {
        static CodeManagerDbContext()
        {
            NpgsqlConnection.GlobalTypeMapper.MapEnum<Roles>();
            NpgsqlConnection.GlobalTypeMapper.MapEnum<JobContext>();
            NpgsqlConnection.GlobalTypeMapper.MapEnum<Permissions>();
        }

        public CodeManagerDbContext(DbContextOptions<CodeManagerDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }
        
        protected CodeManagerDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) {}

        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Run> Runs { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Step> Steps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<Roles>();
            modelBuilder.HasPostgresEnum<JobContext>();
            modelBuilder.HasPostgresEnum<Permissions>();

            modelBuilder.Entity<Project>()
                .HasMany(project => project.Runs)
                .WithOne(run => run.Project)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeamMember>()
                .HasOne(teamMember => teamMember.Team)
                .WithMany(team => team.Members);

            modelBuilder.Entity<Team>()
                .HasMany(team => team.Projects)
                .WithOne(project => project.Team)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Run>()
                .HasMany(run => run.Jobs)
                .WithOne(job => job.Run)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Job>()
                .HasMany(job => job.Steps)
                .WithOne(step => step.Job)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}