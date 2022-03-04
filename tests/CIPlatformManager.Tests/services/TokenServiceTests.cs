using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using CIPlatform.Data.Extensions;
using CIPlatform.Data.JsonWebTokens;
using CIPlatformManager.Configuration;
using CIPlatformManager.Services.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;

namespace CIPlatformManager.Tests
{
    public class TokenServiceTests
    {
        [Test]
        public async Task CreateJobTokenAsync_WithValidClaims_ShouldContainRequiredClaims()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var tokenServiceConfiguration = fixture.Create<IOptions<TokenServiceConfiguration>>();
            tokenServiceConfiguration.Value.JobTokenConfiguration = new TokenConfiguration
            {
                Secret = "129869391af4d6cf84551863b5c9007c",
                LifeTime = 60
            };
            var runId = 0;
            var jobId = 0;
            var runClaim = new Claim(CustomJwtRegisteredClaimNames.RunId, runId.ToString());
            var jobClaim = new Claim(CustomJwtRegisteredClaimNames.JobId, jobId.ToString());
            var jtiClaim = new Claim(JwtRegisteredClaimNames.Jti, string.Empty);
            var expClaim = new Claim(JwtRegisteredClaimNames.Exp, string.Empty);
            var tokenService = new TokenService(tokenServiceConfiguration);

            // Act
            var result = await tokenService.CreateJobTokenAsync(runId, jobId);

            // Assert
            Assert.NotNull(result.Claims.FirstOrDefault(x => x.Type == runClaim.Type && x.Value == runClaim.Value));
            Assert.NotNull(result.Claims.FirstOrDefault(x => x.Type == jobClaim.Type && x.Value == jobClaim.Value));
            Assert.NotNull(result.Claims.FirstOrDefault(x => x.Type == jtiClaim.Type && Guid.TryParse(x.Value, out _)));
            Assert.NotNull(result.Claims.FirstOrDefault(x => x.Type == expClaim.Type));
        }

        [Test]
        public async Task VerifyTokenAsync_WithValidToken_ShouldContainRequiredClaims()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var tokenServiceConfiguration = fixture.Create<IOptions<TokenServiceConfiguration>>();
            tokenServiceConfiguration.Value.JobTokenConfiguration = new TokenConfiguration
            {
                Secret = "129869391af4d6cf84551863b5c9007c",
                LifeTime = 60
            };
            var runId = 0;
            var jobId = 0;
            var runClaim = new Claim(CustomJwtRegisteredClaimNames.RunId, runId.ToString());
            var jobClaim = new Claim(CustomJwtRegisteredClaimNames.JobId, jobId.ToString());
            var jtiClaim = new Claim(JwtRegisteredClaimNames.Jti, string.Empty);
            var expClaim = new Claim(JwtRegisteredClaimNames.Exp, string.Empty);
            var tokenService = new TokenService(tokenServiceConfiguration);
            var validToken = (await tokenService.CreateJobTokenAsync(runId, jobId)).ToBase64String();
            // Act
            var result = await tokenService.VerifyJobTokenAsync(validToken);

            // Assert
            Assert.NotNull(result.HasClaim(x => x.Type == runClaim.Type && x.Value == runClaim.Value));
            Assert.NotNull(result.HasClaim(x => x.Type == jobClaim.Type && x.Value == jobClaim.Value));
            Assert.NotNull(result.HasClaim(x => x.Type == jtiClaim.Type && Guid.TryParse(x.Value, out _)));
            Assert.NotNull(result.HasClaim(x => x.Type == expClaim.Type));
        }
        
        [Test]
        public async Task VerifyTokenAsync_InvalidToken_ShouldThrowSecurityTokenSignatureKeyNotFoundException()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var tokenServiceConfiguration = fixture.Create<IOptions<TokenServiceConfiguration>>();
            tokenServiceConfiguration.Value.JobTokenConfiguration = new TokenConfiguration
            {
                Secret = "129869391af4d6cf84551863b5c9007c",
                LifeTime = 60
            };
            var runId = 0;
            var jobId = 0;
            var tokenService = new TokenService(tokenServiceConfiguration);
            var invalidToken = (await tokenService.CreateJobTokenAsync(runId, jobId)).ToBase64String() + "x";
            
            // Act and Assert
            Assert.ThrowsAsync<SecurityTokenSignatureKeyNotFoundException>(() => tokenService.VerifyJobTokenAsync(invalidToken));
        }
    }
}