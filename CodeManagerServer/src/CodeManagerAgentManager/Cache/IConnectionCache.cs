using StackExchange.Redis;

namespace CodeManagerAgentManager.Cache
{
    public interface IConnectionCache
    {
        public IDatabase Database { get; }
    }
}