using System.Threading.Tasks;

namespace CodeManagerAgent.Hubs.Consumers
{
    public interface IConsumer<T>
        where T : new()
    {
        public Task Consume(T message);
    }
}