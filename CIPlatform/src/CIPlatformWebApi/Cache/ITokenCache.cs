using StackExchange.Redis;

namespace CIPlatformWebApi.Cache
{
    public interface ITokenCache
    {
        public IDatabase Database { get; }
    }
}