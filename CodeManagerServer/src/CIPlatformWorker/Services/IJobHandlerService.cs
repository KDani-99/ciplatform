using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Events;

namespace CIPlatformWorker.Services
{
    public interface IJobHandlerService : IAsyncDisposable, IDisposable
    {
        public Task PrepareEnvironmentAsync();
        public Task ExecuteStepAsync(ChannelWriter<string> channelWriter, StepConfiguration step, int stepIndex);
    }
}