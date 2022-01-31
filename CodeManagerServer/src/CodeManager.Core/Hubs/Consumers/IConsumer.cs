using System.Threading;
using System.Threading.Tasks;

namespace CodeManager.Core.Hubs.Consumers
{
    public interface IConsumer<T>
        where T : new()
    {
        public Task ConsumeAsync(T message, CancellationToken cancellationToken = default);
    }
}