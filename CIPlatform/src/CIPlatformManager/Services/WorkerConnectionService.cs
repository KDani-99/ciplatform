using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Entities;
using CIPlatformManager.Repositories;
using CIPlatformManager.WebSocket.Hubs;
using IPlatformManager.WebSocket;
using Microsoft.AspNetCore.SignalR;

namespace CIPlatformManager.Services
{
    public class WorkerConnectionService : IWorkerConnectionService
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IWorkerConnectionRepository _workerConnectionRepository;

        public WorkerConnectionService(JsonSerializerOptions jsonSerializerOptions,
                                       IWorkerConnectionRepository workerConnectionRepository)
        {
            _jsonSerializerOptions =
                jsonSerializerOptions ?? throw new ArgumentNullException(nameof(jsonSerializerOptions));
            _workerConnectionRepository = workerConnectionRepository ??
                throw new ArgumentNullException(nameof(workerConnectionRepository));
        }

        public async Task<WorkerConnectionData> GetWorkerConnectionAsync(string connectionId)
        {
            var jsonString = await _workerConnectionRepository.GetAsync(connectionId);

            return JsonSerializer.Deserialize<WorkerConnectionData>(jsonString, _jsonSerializerOptions);
        }

        public async Task AddWorkerConnectionOfTypeAsync(WorkerConnectionData workerConnectionData)
        {
            var serialized = JsonSerializer.Serialize(workerConnectionData, _jsonSerializerOptions);

            await _workerConnectionRepository.AddToPoolAsync(workerConnectionData.JobContext,
                                                             workerConnectionData.ConnectionId);
            await _workerConnectionRepository.AddAsync(workerConnectionData.ConnectionId, serialized);
        }

        public async Task RemoveWorkerConnectionAsync(string connectionId)
        {
            var jsonString = await _workerConnectionRepository.GetAsync(connectionId);

            if (jsonString is null)
            {
                return;
            }
            
            var workerConnectionData =
                JsonSerializer.Deserialize<WorkerConnectionData>(jsonString, _jsonSerializerOptions);
            await _workerConnectionRepository.RemoveFromPoolAsync(workerConnectionData!.JobContext, connectionId);
            await _workerConnectionRepository.RemoveAsync(connectionId);
        }

        public async Task UpdateWorkerConnectionAsync(WorkerConnectionData workerConnectionData)
        {
            var jsonString = await _workerConnectionRepository.GetAsync(workerConnectionData.ConnectionId);
            var stored =
                JsonSerializer.Deserialize<WorkerConnectionData>(
                    jsonString, _jsonSerializerOptions);

            workerConnectionData.JobContext = stored!.JobContext;

            var serialized = JsonSerializer.Serialize(workerConnectionData, _jsonSerializerOptions);
            await _workerConnectionRepository.UpdateAsync(workerConnectionData.ConnectionId, serialized);
        }

        public Task<IEnumerable<string>> GetAvailableWorkerConnectionIdsOfTypeAsync(JobContext jobContext)
        {
            return _workerConnectionRepository.GetAllAsync(jobContext);
        }

        public async Task KeepWorkerConnectionAsync(string connectionId)
        {
            var jsonString = await _workerConnectionRepository.GetAsync(connectionId);
            var stored =
                JsonSerializer.Deserialize<WorkerConnectionData>(
                    jsonString, _jsonSerializerOptions);
            
            stored!.LastPing = DateTime.Now;
            
            var serialized = JsonSerializer.Serialize(stored, _jsonSerializerOptions);
            await _workerConnectionRepository.UpdateAsync(connectionId, serialized);
        }

        public Task<IEnumerable<WorkerConnectionData>> GetAvailableWorkerConnectionsAsync()
        {
            throw new NotImplementedException();
        }
    }
}