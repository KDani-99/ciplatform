using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.Controllers;
using CIPlatformWebApi.DataTransfer.Job;
using CIPlatformWebApi.DataTransfer.Run;
using CIPlatformWebApi.Exceptions;
using CIPlatformWebApi.Services.Run;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace CIPlatformWebAPI.Tests.Controllers
{
    public class RunControllerTests
    {
        [Test]
        public async Task GetRunAsync_NoErrors_ShouldReturnValidResponse()
        {
            // Arrange
            var runId = 0;
            var userEntity = new UserEntity();
            var runDataDto = new RunDataDto();
            
            var runService = new Mock<IRunService>();
            runService.Setup(x => x.GetRunDataAsync(It.IsAny<long>(), It.IsAny<UserEntity>()))
                .Returns(Task.FromResult(runDataDto));
            
            var runController = new RunController(runService.Object);
            runController.ControllerContext = new Mock<ControllerContext>().Object;
            runController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            var expectedStatusCode = HttpStatusCode.OK;

            // Act
            var result = await runController.GetRunAsync(runId);
            var response = result as OkObjectResult;
            
            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            Assert.AreEqual(typeof(RunDataDto), response.Value.GetType());
            runService.Verify(x => x.GetRunDataAsync(It.IsAny<long>(), It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task GetRunAsync_UnexpectedError_ShouldReturnErrorResponse()
        {
            // Arrange
            var runId = 0;
            var userEntity = new UserEntity();

            var runService = new Mock<IRunService>();
            runService.Setup(x => x.GetRunDataAsync(It.IsAny<long>(), It.IsAny<UserEntity>()))
                .Throws(new UnauthorizedAccessWebException(string.Empty));
            
            var runController = new RunController(runService.Object);
            runController.ControllerContext = new Mock<ControllerContext>().Object;
            runController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            var expectedStatusCode = HttpStatusCode.Unauthorized;

            // Act
            var result = await runController.GetRunAsync(runId);
            var response = result as ObjectResult;
            
            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int)expectedStatusCode, response.StatusCode);
            runService.Verify(x => x.GetRunDataAsync(It.IsAny<long>(), It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task GetJobAsync_NoErrors_ShouldReturnValidResponse()
        {
            // Arrange
            var runId = 0;
            var jobId = 0;
            var userEntity = new UserEntity();
            var jobDataDto = new JobDataDto();
            
            var runService = new Mock<IRunService>();
            runService.Setup(x => x.GetJobAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<UserEntity>()))
                .Returns(Task.FromResult(jobDataDto));
            
            var runController = new RunController(runService.Object);
            runController.ControllerContext = new Mock<ControllerContext>().Object;
            runController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            var expectedStatusCode = HttpStatusCode.OK;

            // Act
            var result = await runController.GetJobAsync(runId, jobId);
            var response = result as OkObjectResult;
            
            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            Assert.AreEqual(typeof(JobDataDto), response.Value.GetType());
            runService.Verify(x => x.GetJobAsync(It.IsAny<long>(),It.IsAny<long>(), It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task GetJobAsync_UnexpectedError_ShouldReturnErrorResponse()
        {
            // Arrange
            var runId = 0;
            var jobId = 0;
            var userEntity = new UserEntity();

            var runService = new Mock<IRunService>();
            runService.Setup(x => x.GetJobAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<UserEntity>()))
                .Throws(new UnauthorizedAccessWebException(string.Empty));
            
            var runController = new RunController(runService.Object);
            runController.ControllerContext = new Mock<ControllerContext>().Object;
            runController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            var expectedStatusCode = HttpStatusCode.Unauthorized;

            // Act
            var result = await runController.GetJobAsync(runId, jobId);
            var response = result as ObjectResult;
            
            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            runService.Verify(x => x.GetJobAsync(It.IsAny<long>(),It.IsAny<long>(), It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task GetStepFileAsync_NoErrors_ShouldReturnValidResponse()
        {
            // Arrange
            var runId = 0;
            var jobId = 0;
            var stepId = 0;
            var userEntity = new UserEntity();
            
            await using var stream = new MemoryStream();
            
            var runService = new Mock<IRunService>();
            runService.Setup(x => x.GetStepFileStreamAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<UserEntity>()))
                .Returns(Task.FromResult((Stream) stream));
            
            var runController = new RunController(runService.Object);
            runController.ControllerContext = new Mock<ControllerContext>().Object;
            runController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };

            // Act
            var result = await runController.GetStepFileAsync(runId, jobId, stepId);
            var response = result as FileStreamResult;
            
            // Assert
            Assert.NotNull(response);
            Assert.NotNull(response.FileStream);
            runService.Verify(x => x.GetStepFileStreamAsync(It.IsAny<long>(),It.IsAny<long>(),It.IsAny<long>(), It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task GetStepFileAsync_UnexpectedError_ShouldReturnErrorResponse()
        {
            // Arrange
            var runId = 0;
            var jobId = 0;
            var stepId = 0;
            var userEntity = new UserEntity();

            var runService = new Mock<IRunService>();
            runService.Setup(x => x.GetStepFileStreamAsync(It.IsAny<long>(),It.IsAny<long>(), It.IsAny<long>(), It.IsAny<UserEntity>()))
                .Throws(new UnauthorizedAccessWebException(string.Empty));
            
            var runController = new RunController(runService.Object);
            runController.ControllerContext = new Mock<ControllerContext>().Object;
            runController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            var expectedStatusCode = HttpStatusCode.Unauthorized;

            // Act
            var result = await runController.GetStepFileAsync(runId, jobId, stepId);
            var response = result as ObjectResult;
            
            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            runService.Verify(x => x.GetStepFileStreamAsync(It.IsAny<long>(),It.IsAny<long>(),It.IsAny<long>(), It.IsAny<UserEntity>()));
        }

        [Test]
        public async Task DeleteRunAsync_NoErrors_ShouldReturnValidResponse()
        {
            // Arrange
            var runId = 0;
            var userEntity = new UserEntity();
            
            await using var stream = new MemoryStream();
            
            var runService = new Mock<IRunService>();

            var runController = new RunController(runService.Object);
            runController.ControllerContext = new Mock<ControllerContext>().Object;
            runController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            var expectedStatusCode = HttpStatusCode.NoContent;
            
            // Act
            var result = await runController.DeleteRunAsync(runId);
            var response = result as NoContentResult;
            
            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int)expectedStatusCode, response.StatusCode);
            runService.Verify(x => x.DeleteRunAsync(It.IsAny<long>(), It.IsAny<UserEntity>()));
        }
        
        [Test]
        public async Task DeleteRunAsync_UnexpectedError_ShouldReturnErrorResponse()
        {
            // Arrange
            var runId = 0;
            var userEntity = new UserEntity();

            var runService = new Mock<IRunService>();
            runService.Setup(x => x.DeleteRunAsync(It.IsAny<long>(), It.IsAny<UserEntity>()))
                .Throws(new UnauthorizedAccessWebException(string.Empty));
            
            var runController = new RunController(runService.Object);
            runController.ControllerContext = new Mock<ControllerContext>().Object;
            runController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                Items =
                {
                    { "user", userEntity }
                }
            };
            var expectedStatusCode = HttpStatusCode.Unauthorized;

            // Act
            var result = await runController.DeleteRunAsync(runId);
            var response = result as ObjectResult;
            
            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
            runService.Verify(x => x.DeleteRunAsync(It.IsAny<long>(), It.IsAny<UserEntity>()));
        }
    }
}