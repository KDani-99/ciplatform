using CodeManager.Data.Configuration;
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
        }

        public CodeManagerDbContext(DbContextOptions<CodeManagerDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }
        
        protected CodeManagerDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) {}

        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<LoginHistory> LoginHistories { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Run> Runs { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Step> Steps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<Roles>();
            modelBuilder.HasPostgresEnum<JobContext>();
            
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

            modelBuilder.Entity<Run>()
                .HasMany(run => run.Jobs)
                .WithOne(job => job.Run);

            modelBuilder.Entity<Job>()
                .HasMany(job => job.Steps)
                .WithOne(step => step.Job);
        }
    }
}