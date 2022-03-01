using System;
using System.Net;
using Castle.Core.Logging;
using CIPlatformWebApi.Controllers;
using CIPlatformWebApi.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CIPlatformWebAPI.Tests.Controllers
{
    public class ErrorControllerTests
    {
        [Test]
        public void HandleError_BadHttpRequestException_ShouldReturnErrorResponse()
        {
            // Arrange
            var logger = new Mock<ILogger<ErrorController>>();
            var errorController = new ErrorController(logger.Object);

            var feature = new Mock<IExceptionHandlerPathFeature>();
            feature.SetupGet(x => x.Error)
                .Returns(new UnauthorizedAccessWebException(string.Empty));

            var features = new Mock<IFeatureCollection>();
            features.Setup(x => x.Get<IExceptionHandlerPathFeature>())
                .Returns(feature.Object);
            
            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(x => x.Features)
                .Returns(features.Object);
            
            errorController.ControllerContext = new Mock<ControllerContext>().Object;
            errorController.ControllerContext.HttpContext = httpContext.Object;
            var expectedStatusCode = HttpStatusCode.Unauthorized;

            // Act
            var result = errorController.HandleError();
            var response = result as ObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
        }
        
        [Test]
        public void HandleError_UnexpectedError_ShouldReturnErrorResponse()
        {
            // Arrange
            var logger = new Mock<ILogger<ErrorController>>();
            var errorController = new ErrorController(logger.Object);

            var feature = new Mock<IExceptionHandlerPathFeature>();
            feature.SetupGet(x => x.Error)
                .Returns(new ArgumentNullException(string.Empty));

            var features = new Mock<IFeatureCollection>();
            features.Setup(x => x.Get<IExceptionHandlerPathFeature>())
                .Returns(feature.Object);
            
            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(x => x.Features)
                .Returns(features.Object);
            
            errorController.ControllerContext = new Mock<ControllerContext>().Object;
            errorController.ControllerContext.HttpContext = httpContext.Object;
            var expectedStatusCode = HttpStatusCode.InternalServerError;

            // Act
            var result = errorController.HandleError();
            var response = result as ObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual((int) expectedStatusCode, response.StatusCode);
        }
    }
}