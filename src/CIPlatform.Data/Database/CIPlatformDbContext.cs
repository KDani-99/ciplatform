using CIPlatform.Data.Configuration;
using CIPlatform.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CIPlatform.Data.Database
{
    public class CIPlatformDbContext : DbContext
    {
        static CIPlatformDbContext()
        {
            NpgsqlConnection.GlobalTypeMapper.MapEnum<Roles>();
            NpgsqlConnection.GlobalTypeMapper.MapEnum<JobContext>();
            NpgsqlConnection.GlobalTypeMapper.MapEnum<Permissions>();
        }

        public CIPlatformDbContext(DbContextOptions<CIPlatformDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        protected CIPlatformDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<TeamEntity> Teams { get; set; }
        public DbSet<TeamMemberEntity> TeamMembers { get; set; }
        public DbSet<ProjectEntity> Projects { get; set; }
        public DbSet<RunEntity> Runs { get; set; }
        public DbSet<JobEntity> Jobs { get; set; }
        public DbSet<StepEntity> Steps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<Roles>();
            modelBuilder.HasPostgresEnum<JobContext>();
            modelBuilder.HasPostgresEnum<Permissions>();

            modelBuilder.Entity<ProjectEntity>()
                        .HasMany(project => project.Runs)
                        .WithOne(run => run.Project)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeamMemberEntity>()
                        .HasOne(teamMember => teamMember.Team)
                        .WithMany(team => team.Members);

            modelBuilder.Entity<TeamEntity>()
                        .HasMany(team => team.Projects)
                        .WithOne(project => project.Team)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RunEntity>()
                        .HasMany(run => run.Jobs)
                        .WithOne(job => job.Run)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JobEntity>()
                        .HasMany(job => job.Steps)
                        .WithOne(step => step.Job)
                        .OnDelete(DeleteBehavior.Cascade);
        }
    }
}