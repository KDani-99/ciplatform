﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeManager.Data.Configuration;
using CodeManager.Data.Entities;
using CodeManagerAgentManager.Cache;
using Microsoft.OpenApi.Extensions;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace CodeManagerAgentManager.Repositories
{
    public class WorkerConnectionRepository : IWorkerConnectionRepository
    {
        private readonly IConnectionCache _connectionCache;
        private const string ConnectionsHashKey = "connections";
        
        public WorkerConnectionRepository(IConnectionCache connectionCache)
        {
            _connectionCache = connectionCache ?? throw new ArgumentNullException(nameof(connectionCache));
        }

        public async Task<string> GetAsync(string connectionId)
        {
            var result = await _connectionCache.Database.HashGetAsync(ConnectionsHashKey, connectionId);
            return result.ToString();
        }

        public Task<bool> AddAsync(string connectionId, string workerData)
        {
            // associate jobContext name with connection in the database
            // TODO: below it just stores info about the worker  
            return _connectionCache.Database.HashSetAsync(ConnectionsHashKey, connectionId, workerData);
        }
        
        public Task<bool> AddToPoolAsync(JobContext jobContext, string connectionId)
        {
            // TODO: it allows fast lookup - > only available agents will be there
            return _connectionCache.Database.SetAddAsync(jobContext.GetDisplayName(), connectionId);
        }

        public Task<bool> RemoveAsync(string connectionId)
        {
            return _connectionCache.Database.HashDeleteAsync(ConnectionsHashKey, connectionId);
        }

        public Task<bool> RemoveFromPoolAsync(JobContext jobContext, string connectionId)
        {
            return _connectionCache.Database.SetRemoveAsync(jobContext.GetDisplayName(), connectionId);
        }

        public Task<bool> UpdateAsync(string connectionId, string workerData)
        {
            return AddAsync(connectionId, workerData);
        }

        public async Task<IEnumerable<string>> GetAsync(JobContext jobContext)
        {
            var workerConnections = await _connectionCache.Database.SetMembersAsync(jobContext.GetDisplayName());

            return workerConnections.Select(connection => connection.ToString());
        }
    }
}