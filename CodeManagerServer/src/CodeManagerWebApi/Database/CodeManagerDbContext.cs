using System;
using CodeManager.Data.Entities;
using CodeManagerWebApi.Configuration;
using CodeManagerWebApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;

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

            if (serviceAccountConfiguration.IsEnabled)
                modelBuilder.Entity<User>()
                            .HasData(new User
                            {
                                Id = 1,
                                Email = "admin.site@noreply",
                                Username = serviceAccountConfiguration.Username,
                                Password = credentialManagerService.CreateHashedPassword(
                                    serviceAccountConfiguration.Password),
                                RegistrationTimestamp = DateTime.Now,
                                Name = "Admin",
                                Roles = new[] {Roles.Admin, Roles.User}
                            });
        }
    }
}