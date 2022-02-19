using StackExchange.Redis;

namespace CIPlatformManager.Cache
{
    public interface ICache
    {
        public IDatabase Database { get; }
    }
}