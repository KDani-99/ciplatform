using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CIPlatform.Data.Agent;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Entities;
using CIPlatformManager.Repositories.Workers;
using CIPlatformManager.Services.Workers;
using Moq;
using NUnit.Framework;

namespace CIPlatformManager.Tests.Services
{
    public class WorkerConnectionServiceTests
    {
        [Test]
        public async Task GetWorkerConnectionAsync_ValidConnectionId_ShouldReturnWorkerConnectionDataEntityInstance()
        {
            // Arrange
            var jsonString = "{\"ConnectionId\": \"abc-123\",\"JobContext\": \"Docker\",\"WorkerState\": \"Available\"}";
            var connectionId = "abc-123";
            
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var workerConnectionRepository = new Mock<IWorkerConnectionRepository>();
            workerConnectionRepository.Setup(x => x.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(jsonString));
            
            var workerConnectionService = new WorkerConnectionService(jsonSerializerOptions, workerConnectionRepository.Object);

            // Act
            var result = await workerConnectionService.GetWorkerConnectionAsync(connectionId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(connectionId, result.ConnectionId);
            workerConnectionRepository.Verify(x => x.GetAsync(It.IsAny<string>()));
        }
        
        [Test]
        public async Task AddWorkerConnectionOfTypeAsync_ValidWorkerConnection_ShouldHandleProperly()
        {
            // Arrange
            var workerConnectionData = new WorkerConnectionDataEntity();

            var jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var workerConnectionRepository = new Mock<IWorkerConnectionRepository>();

            var workerConnectionService = new WorkerConnectionService(jsonSerializerOptions, workerConnectionRepository.Object);

            // Act
            await workerConnectionService.AddWorkerConnectionOfTypeAsync(workerConnectionData);

            // Assert
            workerConnectionRepository.Verify(x => x.AddToPoolAsync(It.IsAny<JobContext>(), It.IsAny<string>()));
            workerConnectionRepository.Verify(x => x.AddAsync(It.IsAny<string>(), It.IsAny<string>()));
        }
        
        [Test]
        public async Task RemoveWorkerConnectionAsync_ValidConnectionId_ShouldHandleProperly()
        {
            // Arrange
            var jsonString = "{\"ConnectionId\": \"abc-123\",\"JobContext\": \"Docker\",\"WorkerState\": \"Available\"}";
            var connectionId = "abc-123";

            var jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var workerConnectionRepository = new Mock<IWorkerConnectionRepository>();
            workerConnectionRepository.Setup(x => x.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(jsonString));

            var workerConnectionService = new WorkerConnectionService(jsonSerializerOptions, workerConnectionRepository.Object);

            // Act
            await workerConnectionService.RemoveWorkerConnectionAsync(connectionId);

            // Assert
            workerConnectionRepository.Verify(x => x.RemoveFromPoolAsync(It.IsAny<JobContext>(), It.IsAny<string>()));
            workerConnectionRepository.Verify(x => x.RemoveAsync(It.IsAny<string>()));
        }
        
        [Test]
        public async Task UpdateWorkerConnectionAsync_AvailableState_ShouldAddToPoolAsync()
        {
            // Arrange
            var workerConnectionData = new WorkerConnectionDataEntity();
            var jsonString = "{\"ConnectionId\": \"abc-123\",\"JobContext\": \"Docker\",\"WorkerState\": \"Available\"}";

            var jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var workerConnectionRepository = new Mock<IWorkerConnectionRepository>();
            workerConnectionRepository.Setup(x => x.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(jsonString));

            var workerConnectionService = new WorkerConnectionService(jsonSerializerOptions, workerConnectionRepository.Object);

            // Act
            await workerConnectionService.UpdateWorkerConnectionAsync(workerConnectionData);

            // Assert
            workerConnectionRepository.Verify(x => x.AddToPoolAsync(It.IsAny<JobContext>(), It.IsAny<string>()));
            workerConnectionRepository.Verify(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<string>()));
        }
        
        [Test]
        public async Task UpdateWorkerConnectionAsync_NotAvailableState_ShouldRemoveFromPoolAsync()
        {
            // Arrange
            var workerConnectionData = new WorkerConnectionDataEntity
            {
                WorkerState = WorkerState.Working
            };
            var jsonString = "{\"ConnectionId\": \"abc-123\",\"JobContext\": \"Docker\",\"WorkerState\": \"Available\"}";

            var jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var workerConnectionRepository = new Mock<IWorkerConnectionRepository>();
            workerConnectionRepository.Setup(x => x.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(jsonString));

            var workerConnectionService = new WorkerConnectionService(jsonSerializerOptions, workerConnectionRepository.Object);

            // Act
            await workerConnectionService.UpdateWorkerConnectionAsync(workerConnectionData);

            // Assert
            workerConnectionRepository.Verify(x => x.RemoveFromPoolAsync(It.IsAny<JobContext>(), It.IsAny<string>()));
            workerConnectionRepository.Verify(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<string>()));
        }
        
        [Test]
        public async Task DequeueAvailableWorkerConnectionOfTypeAsync_ValidJobContext_ShouldRemoveFromPoolAsync()
        {
            // Arrange
            var jobContext = JobContext.Docker;
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var workerConnectionRepository = new Mock<IWorkerConnectionRepository>();

            var workerConnectionService = new WorkerConnectionService(jsonSerializerOptions, workerConnectionRepository.Object);

            // Act
            await workerConnectionService.DequeueAvailableWorkerConnectionOfTypeAsync(jobContext);

            // Assert
            workerConnectionRepository.Verify(x => x.RemoveFromPoolAsync(It.IsAny<JobContext>()));
        }
        
        [Test]
        public async Task GetAvailableWorkerConnectionIdsOfTypeAsync_ValidJobContext_ShouldGetAllAsync()
        {
            // Arrange
            var jobContext = JobContext.Docker;
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var workerConnectionRepository = new Mock<IWorkerConnectionRepository>();

            var workerConnectionService = new WorkerConnectionService(jsonSerializerOptions, workerConnectionRepository.Object);

            // Act
            await workerConnectionService.GetAvailableWorkerConnectionIdsOfTypeAsync(jobContext);

            // Assert
            workerConnectionRepository.Verify(x => x.GetAllAsync(It.IsAny<JobContext>()));
        }
        
        [Test]
        public async Task KeepWorkerConnectionAsync_ValidDetails_ShouldUpdateAsync()
        {
            // Arrange
            var connectionId = "abc-123";
            var jsonString = "{\"ConnectionId\": \"abc-123\",\"JobContext\": \"Docker\",\"WorkerState\": \"Available\"}";

            var jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var workerConnectionRepository = new Mock<IWorkerConnectionRepository>();
            workerConnectionRepository.Setup(x => x.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(jsonString));

            var workerConnectionService = new WorkerConnectionService(jsonSerializerOptions, workerConnectionRepository.Object);

            // Act
            await workerConnectionService.KeepWorkerConnectionAsync(connectionId);

            // Assert
            workerConnectionRepository.Verify(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<string>()));
        }
        
        [Test]
        public async Task MarkWorkerConnectionAsAvailableAsync_ValidJobContext_GetAllAsync()
        {
            // Arrange
            var connectionId = "abc-123";
            var jsonString = "{\"ConnectionId\": \"abc-123\",\"JobContext\": \"Docker\",\"WorkerState\": \"Available\"}";

            var jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var workerConnectionRepository = new Mock<IWorkerConnectionRepository>();
            workerConnectionRepository.Setup(x => x.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(jsonString));

            var workerConnectionService = new WorkerConnectionService(jsonSerializerOptions, workerConnectionRepository.Object);

            // Act
            await workerConnectionService.MarkWorkerConnectionAsAvailableAsync(connectionId);

            // Assert
            workerConnectionRepository.Verify(x => x.GetAsync( It.IsAny<string>()));
            workerConnectionRepository.Verify(x => x.RemoveFromPoolAsync(It.IsAny<JobContext>(), It.IsAny<string>()));
            workerConnectionRepository.Verify(x => x.AddToPoolAsync(It.IsAny<JobContext>(), It.IsAny<string>()));
            workerConnectionRepository.Verify(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<string>()));
        }
    }
}