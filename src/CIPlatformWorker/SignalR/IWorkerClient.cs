using System.Threading.Channels;
using System.Threading.Tasks;
using CIPlatform.Data.Events;
using Microsoft.AspNetCore.SignalR.Client;

namespace CIPlatformWorker.SignalR
{
    public interface IWorkerClient
    {
        public HubConnection HubConnection { get; }

        public Task ConfigureAsync();
        public Task StreamLogAsync(long runId, long jobId, long stepIndex, ChannelReader<string> channelReader);
        public Task SendStepResultAsync(StepResultEvent stepResultEvent);
        public Task PingAsync();
        public Task FinishJobAsync();
    }
}