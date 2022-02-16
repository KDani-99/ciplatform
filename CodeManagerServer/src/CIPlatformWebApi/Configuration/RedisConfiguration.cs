namespace CIPlatformWebApi.Configuration
{
    public class RedisConfiguration
    {
        public string ConnectionString { get; set; }
        public int Database { get; set; }
        public string Password { get; set; }
    }
}