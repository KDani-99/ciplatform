using System.Linq;
using System.Threading.Tasks;
using CIPlatformWebApi.Configuration;
using CIPlatformWebApi.Exceptions;
using CIPlatformWebApi.Services.FileProcessor;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CIPlatformWebAPI.Tests.Services
{
    public class FileProcessorServiceTests
    {
        [Test]
        public void ProcessAsync_ShortStepName_ShouldThrowInvalidInstructionFileException()
        {
            // Arrange
            var data = @"
            jobs:
              test:
                context: 'windows'
                steps:
                  - name:
                    cmd: 'echo test'  
            ";
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(LowerCaseNamingConvention.Instance)
                .Build();
            var options = Options.Create(new YmlConfiguration());
            var fileProcessorService = new YmlFileProcessorService(deserializer, options);
            
            // Act and Assert
            Assert.ThrowsAsync<InvalidInstructionFileException>(() => fileProcessorService.ProcessAsync(data));
        }
        
        [Test]
        public void ProcessAsync_ShortCmd_ShouldThrowInvalidInstructionFileException()
        {
            // Arrange
            var data = @"
            jobs:
              test:
                context: 'windows'
                steps:
                  - name: 'test'
                    cmd: 
            ";
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(LowerCaseNamingConvention.Instance)
                .Build();
            var options = Options.Create(new YmlConfiguration());
            var fileProcessorService = new YmlFileProcessorService(deserializer, options);
            
            // Act and Assert
            Assert.ThrowsAsync<InvalidInstructionFileException>(() => fileProcessorService.ProcessAsync(data));
        }
        
        [Test]
        public void ProcessAsync_TooManySteps_ShouldThrowInvalidInstructionFileException()
        {
            // Arrange
            var data = @"
            jobs:
              test:
                context: 'windows'
                steps:
                  - name: 'test'
                    cmd: 'echo test'
            ";
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(LowerCaseNamingConvention.Instance)
                .Build();
            var options = Options.Create(new YmlConfiguration
            {
                MaxStepPerJobCount = 0
            });
            var fileProcessorService = new YmlFileProcessorService(deserializer, options);
            
            // Act and Assert
            Assert.ThrowsAsync<InvalidInstructionFileException>(() => fileProcessorService.ProcessAsync(data));
        }
        
        [Test]
        public void ProcessAsync_ShortJobName_ShouldThrowInvalidInstructionFileException()
        {
            // Arrange
            var data = @"
            jobs:
              d:
                context: 'windows'
                steps:
                  - name: 'test'
                    cmd: 'echo test'
            ";
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(LowerCaseNamingConvention.Instance)
                .Build();
            var options = Options.Create(new YmlConfiguration
            {
                MaxStepPerJobCount = 1,
                MaxJobCount = 1
            });
            var fileProcessorService = new YmlFileProcessorService(deserializer, options);
            
            // Act and Assert
            Assert.ThrowsAsync<InvalidInstructionFileException>(() => fileProcessorService.ProcessAsync(data));
        }
        
        [Test]
        public void ProcessAsync_TooManyJobs_ShouldThrowInvalidInstructionFileException()
        {
            // Arrange
            var data = @"
            jobs:
              test:
                context: 'windows'
                steps:
                  - name: 'test'
                    cmd: 'echo test'
            ";
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(LowerCaseNamingConvention.Instance)
                .Build();
            var options = Options.Create(new YmlConfiguration
            {
                MaxJobCount = 0
            });
            var fileProcessorService = new YmlFileProcessorService(deserializer, options);
            
            // Act and Assert
            Assert.ThrowsAsync<InvalidInstructionFileException>(() => fileProcessorService.ProcessAsync(data));
        }
        
        [Test]
        public async Task ProcessAsync_ValidConfig_ShouldReturnRunConfiguration()
        {
            // Arrange
            var data = @"
            jobs:
              test:
                context: 'windows'
                steps:
                  - name: 'test'
                    cmd: 'echo test'
            ";
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(LowerCaseNamingConvention.Instance)
                .Build();
            var options = Options.Create(new YmlConfiguration
            {
                MaxJobCount = 1,
                MaxStepPerJobCount = 1
            });
            var fileProcessorService = new YmlFileProcessorService(deserializer, options);
            
            var expectedJobCount = 1;
            var expectedJobName = "test";

            var expectedStepCount = 1;
            var expectedStepName = "test";
            
            // Act
            var result = await fileProcessorService.ProcessAsync(data);
            
            // Assert
            Assert.NotNull(result);
            Assert.AreEqual( expectedJobCount,result.Jobs.Count);
            Assert.Contains(expectedJobName, result.Jobs.Keys);
            Assert.AreEqual(expectedStepCount, result.Jobs[expectedJobName].Steps.Count);
            Assert.Contains(expectedStepName, result.Jobs[expectedJobName].Steps.Select(step => step.Name).ToList());
        }
    }
}