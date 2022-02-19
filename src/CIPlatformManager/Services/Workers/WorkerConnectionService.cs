using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CIPlatform.Data.Agent;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Entities;
using CIPlatformManager.Repositories;
using CIPlatformManager.Repositories.Workers;
using CIPlatformManager.SignalR.Hubs;
using IPlatformManager.WebSocket;
using Microsoft.AspNetCore.SignalR;

namespace CIPlatformManager.Services.Workers
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

        public async Task<WorkerConnectionDataEntity> GetWorkerConnectionAsync(string connectionId)
        {
            var jsonString = await _workerConnectionRepository.GetAsync(connectionId);

            return JsonSerializer.Deserialize<WorkerConnectionDataEntity>(jsonString, _jsonSerializerOptions);
        }

        public async Task AddWorkerConnectionOfTypeAsync(WorkerConnectionDataEntity workerConnectionData)
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
                JsonSerializer.Deserialize<WorkerConnectionDataEntity>(jsonString, _jsonSerializerOptions);
            await _workerConnectionRepository.RemoveFromPoolAsync(workerConnectionData!.JobContext, connectionId);
            await _workerConnectionRepository.RemoveAsync(connectionId);
        }

        public async Task UpdateWorkerConnectionAsync(WorkerConnectionDataEntity workerConnectionData)
        {
            var jsonString = await _workerConnectionRepository.GetAsync(workerConnectionData.ConnectionId);
            var stored =
                DeserializeWorkerConnectionData(jsonString);

            workerConnectionData.JobContext = stored!.JobContext;

            if (workerConnectionData.WorkerState == WorkerState.Available)
            {
                await _workerConnectionRepository.AddToPoolAsync(workerConnectionData.JobContext, workerConnectionData.ConnectionId);
            }
            else
            {
                await _workerConnectionRepository.RemoveFromPoolAsync(workerConnectionData.JobContext, workerConnectionData.ConnectionId);
            }

            var serialized = JsonSerializer.Serialize(workerConnectionData, _jsonSerializerOptions);
            await _workerConnectionRepository.UpdateAsync(workerConnectionData.ConnectionId, serialized);
        }

        public Task<string> DequeueAvailableWorkerConnectionOfTypeAsync(JobContext jobContext)
        {
            return _workerConnectionRepository.RemoveFromPoolAsync(jobContext);
        }

        public Task QueueWorkerConnectionOfTypeAsync(JobContext jobContext, string connectionId)
        {
            return _workerConnectionRepository.AddToPoolAsync(jobContext, connectionId);
        }

        public Task<IEnumerable<string>> GetAvailableWorkerConnectionIdsOfTypeAsync(JobContext jobContext)
        {
            return _workerConnectionRepository.GetAllAsync(jobContext);
        }

        public async Task KeepWorkerConnectionAsync(string connectionId)
        {
            var jsonString = await _workerConnectionRepository.GetAsync(connectionId);
            var stored =
                JsonSerializer.Deserialize<WorkerConnectionDataEntity>(
                    jsonString, _jsonSerializerOptions);
            
            stored!.LastPing = DateTime.Now;
            
            var serialized = JsonSerializer.Serialize(stored, _jsonSerializerOptions);
            await _workerConnectionRepository.UpdateAsync(connectionId, serialized);
        }

        public async Task MarkWorkerConnectionAsAvailableAsync(string connectionId)
        {
            var workerConnectionData = await GetWorkerConnectionAsync(connectionId);
            workerConnectionData.WorkerState = WorkerState.Available;
            
            await _workerConnectionRepository.RemoveFromPoolAsync(workerConnectionData.JobContext, connectionId);
            await _workerConnectionRepository.AddToPoolAsync(workerConnectionData.JobContext, connectionId);

            await UpdateWorkerConnectionAsync(workerConnectionData);
        }

        private WorkerConnectionDataEntity DeserializeWorkerConnectionData(string data)
        {
            return JsonSerializer.Deserialize<WorkerConnectionDataEntity>(
                data, _jsonSerializerOptions);
        }
    }
}