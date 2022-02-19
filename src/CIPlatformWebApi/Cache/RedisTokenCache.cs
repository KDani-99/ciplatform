using System;
using CIPlatformWebApi.Configuration;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace CIPlatformWebApi.Cache
{
    public class RedisTokenCache : ITokenCache
    {
        private readonly RedisConfiguration _redisConfiguration;
        private IConnectionMultiplexer _connectionMultiplexer;

        public RedisTokenCache(IOptions<RedisConfiguration> redisConfiguration)
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
                EndPoints = { _redisConfiguration.ConnectionString },
                Password = _redisConfiguration.Password,
                DefaultDatabase = _redisConfiguration.Database
            });
        }
    }
}