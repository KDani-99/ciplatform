using System.Threading.Channels;
using System.Threading.Tasks;
using CodeManager.Data.Events;
using Microsoft.AspNetCore.SignalR.Client;

namespace CodeManagerAgent.WebSocket
{
    public interface IWorkerClient
    {
        public HubConnection HubConnection { get; }

        public Task ConfigureAsync();
        public Task StreamLogAsync(long runId, long jobId, long stepIndex, ChannelReader<string> channelReader);
        public Task SendStepResult(StepResultEvent stepResultEvent);
    }
}