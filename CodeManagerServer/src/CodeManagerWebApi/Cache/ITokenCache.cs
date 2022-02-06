using StackExchange.Redis;

namespace CodeManagerWebApi.Cache
{
    public interface ITokenCache
    {
        public IDatabase Database { get; }
    }
}