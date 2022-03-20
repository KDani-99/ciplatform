namespace CIPlatformManager.Configuration
{
    public class RedisConfiguration
    {
        public string ConnectionString { get; set; }
        public string Password { get; set; }
        public RedisCacheConfiguration JobQueueCacheConfiguration { get; set; }
        public RedisCacheConfiguration ConnectionCacheConfiguration { get; set; }
    }
}