using System.Threading;
using System.Threading.Tasks;

namespace CIPlatform.Core.SignalR.Consumers
{
    public interface IConsumer<T>
        where T : new()
    {
        public Task ConsumeAsync(T message, CancellationToken cancellationToken = default);
    }
}