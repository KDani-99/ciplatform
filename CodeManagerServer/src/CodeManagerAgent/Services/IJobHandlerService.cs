using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using CodeManager.Data.Configuration;

namespace CodeManagerAgent.Services
{
    public interface IJobHandlerService : IAsyncDisposable, IDisposable
    {
        public Task PrepareEnvironmentAsync();
        public Task ExecuteStepAsync(ChannelWriter<string> channelWriter, StepConfiguration step, int stepIndex);
    }
}