using System.Threading.Tasks;

namespace CodeManager.Core.Hubs.Consumers
{
    public interface IConsumer<T>
        where T : new()
    {
        public Task Consume(T message);
    }
}