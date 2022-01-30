using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using CodeManager.Data.Configuration;
using CodeManager.Data.Entities;
using CodeManagerAgentManager.Exceptions;
using CodeManagerAgentManager.Repositories;

namespace CodeManagerAgentManager.Services
{
    public class WorkerConnectionService : IWorkerConnectionService
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IWorkerConnectionRepository _workerConnectionRepository;
        
        public WorkerConnectionService(JsonSerializerOptions jsonSerializerOptions, IWorkerConnectionRepository workerConnectionRepository)
        {
            _jsonSerializerOptions =
                jsonSerializerOptions ?? throw new ArgumentNullException(nameof(jsonSerializerOptions));
            _workerConnectionRepository = workerConnectionRepository ??
                                       throw new ArgumentNullException(nameof(workerConnectionRepository));
        }
        
        public async Task AddWorkerConnectionOfTypeAsync(WorkerConnectionData workerConnectionData)
        {
            var serialized = JsonSerializer.Serialize(workerConnectionData, _jsonSerializerOptions);

            await _workerConnectionRepository.AddToPoolAsync(workerConnectionData.JobContext, workerConnectionData.ConnectionId);
            await _workerConnectionRepository.AddAsync(workerConnectionData.ConnectionId, serialized);
        }

        public async Task RemoveWorkerConnectionAsync(string connectionId)
        {
            var jsonString = await _workerConnectionRepository.GetAsync(connectionId);
            var workerConnectionData = JsonSerializer.Deserialize<WorkerConnectionData>(jsonString, _jsonSerializerOptions);
            await _workerConnectionRepository.RemoveFromPoolAsync(workerConnectionData.JobContext, connectionId);
            await _workerConnectionRepository.RemoveAsync(connectionId);
        }

        public async Task UpdateWorkerConnectionAsync(WorkerConnectionData workerConnectionData)
        {
            var jsonString = await _workerConnectionRepository.GetAsync(workerConnectionData.ConnectionId);
            var stored =
                JsonSerializer.Deserialize<WorkerConnectionData>(
                    jsonString, _jsonSerializerOptions);

            workerConnectionData.JobContext = stored.JobContext;
            
            var serialized = JsonSerializer.Serialize(workerConnectionData, _jsonSerializerOptions);
            await _workerConnectionRepository.UpdateAsync(workerConnectionData.ConnectionId, serialized);
        }

        public Task<IEnumerable<string>> GetAvailableWorkerConnectionIdsOfTypeAsync(JobContext jobContext)
        {
            return _workerConnectionRepository.GetAllAsync(jobContext);
        }

        public Task<IEnumerable<WorkerConnectionData>> GetAvailableWorkerConnectionsAsync()
        {
            throw new NotImplementedException();
        }
    }
}