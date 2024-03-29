﻿using System;
using CIPlatformManager.Configuration;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace CIPlatformManager.Cache
{
    public class RedisCache : IRedisConnectionCache
    {
        private readonly RedisConfiguration _redisConfiguration;
        private IConnectionMultiplexer _connectionMultiplexer;

        public RedisCache(IOptions<RedisConfiguration> redisConfiguration)
        {
            _redisConfiguration =
                redisConfiguration.Value ?? throw new ArgumentNullException(nameof(redisConfiguration));
            Setup();
        }

        public IDatabase Database => _connectionMultiplexer.GetDatabase();

        private void Setup()
        {
            _connectionMultiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                EndPoints = {_redisConfiguration.ConnectionString},
                Password = _redisConfiguration.Password,
                DefaultDatabase = _redisConfiguration.JobQueueCacheConfiguration.Database
            });
        }
    }
}