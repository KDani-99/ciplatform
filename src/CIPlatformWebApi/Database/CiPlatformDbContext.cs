using System;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.Configuration;
using CIPlatformWebApi.Services.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace CIPlatformWebApi.Database
{
    public class CiPlatformDbContext : CIPlatform.Data.Database.CIPlatformDbContext
    {
        public CiPlatformDbContext(DbContextOptions<CiPlatformDbContext> dbContextOptions) : base(dbContextOptions)
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
            {
                modelBuilder.Entity<UserEntity>()
                    .HasData(new UserEntity
                    {
                        Id = 1,
                        Email = "admin.site@noreply",
                        Username = serviceAccountConfiguration.Username,
                        Password = credentialManagerService.CreateHashedPassword(
                            serviceAccountConfiguration.Password),
                        RegistrationTimestamp = DateTime.Now,
                        Name = "Admin",
                        Roles = new[] { Roles.Admin, Roles.User }
                    });
            }
        }
    }
}