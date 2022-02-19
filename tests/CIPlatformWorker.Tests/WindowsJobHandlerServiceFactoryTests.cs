﻿using System;
using System.Threading;
using AutoFixture;
using AutoFixture.AutoMoq;
using Castle.Core.Logging;
using CIPlatform.Data.Configuration;
using CIPlatformWorker.Configuration;
using CIPlatformWorker.Entities;
using CIPlatformWorker.Factories.Job;
using CIPlatformWorker.Services.Job;
using CIPlatformWorker.SignalR;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace CIPlatformWorker.Tests
{
public class WindowsJobHandlerServiceFactoryTests
    {
        [Test]
        public void Create_NewWindowsJobHandlerServiceFactory_ShouldThrowArgumentNullException()
        {
            // Arrange
            Assert.Throws<ArgumentNullException>(() => new WindowsJobHandlerServiceFactory(null));
        }

        [Test]
        public void Create_NewWindowsJobHandlerServiceWithCreate_ShouldReturnNewWindowsHandlerServiceInstance()
        {
            // Arrange
            var jobConfiguration = new JobConfiguration();
            var jobDetails = new JobDetails();
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var agentConfiguration = fixture.Create<IOptions<WorkerConfiguration>>();
            var workerClient = new Mock<IWorkerClient>();
            var loggerFactory = new Mock<ILoggerFactory>();
            var cancellationToken = CancellationToken.None;
            var mockWindowsJobHandlerServiceFactory = new WindowsJobHandlerServiceFactory(agentConfiguration);

            // Act and Assert
            Assert.DoesNotThrow(
                () => mockWindowsJobHandlerServiceFactory.Create(jobDetails, jobConfiguration, cancellationToken));
            Assert.NotNull(mockWindowsJobHandlerServiceFactory.Create(jobDetails, jobConfiguration, cancellationToken));
            Assert.IsInstanceOf<WindowsJobHandlerService>(
                mockWindowsJobHandlerServiceFactory.Create(jobDetails, jobConfiguration, cancellationToken));
        }

        [Test]
        public void Create_NewWindowsJobHandlerServiceWithCreate_ShouldThrowArgumentNullException()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var agentConfiguration = fixture.Create<IOptions<WorkerConfiguration>>();
            var workerClient = new Mock<IWorkerClient>();
            var loggerFactory = new Mock<ILoggerFactory>();
            var cancellationToken = CancellationToken.None;
            var mockWindowsJobHandlerServiceFactory = new WindowsJobHandlerServiceFactory(agentConfiguration);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() =>
                                                     mockWindowsJobHandlerServiceFactory.Create(
                                                         null, null, cancellationToken));
        }
    }
}