using CodeManagerWebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodeManagerWebApi.Database
{
    public class CodeManagerDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<Plan> Plans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(user => user.Plan)
                .WithOne();

            modelBuilder.Entity<Team>()
                .HasMany(team => team.Members)
                .WithMany(teamMember => teamMember.User.Teams);

            modelBuilder.Entity<Plan>()
                .HasData(new Plan
                {
                    Name = "Basic",
                    MaxCreatedTeamsPerUser = 5,
                    MaxJoinedTeamsPerUser = 15
                });
        }
        
        
    }
}