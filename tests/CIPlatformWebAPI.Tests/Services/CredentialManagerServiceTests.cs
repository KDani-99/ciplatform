using System;
using CIPlatformWebApi.Services.Auth;
using NUnit.Framework;

namespace CIPlatformWebAPI.Tests.Services
{
    public class CredentialManagerServiceTests
    {
        [Test]
        public void CreateHashedPassword_NotEmptyString_ShouldReturnHashed()
        {
            // Arrange
            var plain = "test";
            var credentialManagerService = new CredentialManagerService();
            
            // Act
            var result = credentialManagerService.CreateHashedPassword(plain);
            
            // Assert
            Assert.NotNull(result);
        }
        
        [Test]
        public void VerifyPassword_NotEmptyString_ShouldBeValid()
        {
            // Arrange
            var plain = "test";
            var credentialManagerService = new CredentialManagerService();
            var expectedPassword = credentialManagerService.CreateHashedPassword(plain);
            
            // Act
            var result = credentialManagerService.VerifyPassword(plain, expectedPassword);
            
            // Assert
            Assert.True(result);
        }
    }
}