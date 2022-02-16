using StackExchange.Redis;

namespace CIPlatformManager.Cache
{
    public interface IConnectionCache
    {
        public IDatabase Database { get; }
    }
}