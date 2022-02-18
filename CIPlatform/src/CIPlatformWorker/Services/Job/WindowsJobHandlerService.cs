using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using CIPlatformWorker.Configuration;
using CIPlatformWorker.Entities;
using CIPlatformWorker.Exceptions;
using CIPlatform.Data.Configuration;
using CIPlatform.Data.Events;
using CIPlatformWorker.Extensions;
using Microsoft.Extensions.Options;

namespace CIPlatformWorker.Services.Job
{
    public class WindowsJobHandlerService : JobHandlerService
    {
        // unit of work
        public WindowsJobHandlerService(
                                        JobConfiguration jobConfiguration,
                                        IOptions<WorkerConfiguration> agentConfiguration,
                                        CancellationToken cancellationToken)
            : base(jobConfiguration, agentConfiguration,
                   cancellationToken)
        {
        }

        public override async Task ExecuteStepAsync(ChannelWriter<string> channelWriter,
                                                    StepConfiguration step,
                                                    int stepIndex)
        {
            await base.ExecuteStepAsync(channelWriter, step, stepIndex);
            
            using var process = new Process();
            process.ConfigureCliProcess("powershell.exe", step.Cmd,
                                        JobConfiguration.Environment);

            process.OutputDataReceived += async (_, eventArgs) =>
                await channelWriter.WriteAsync(eventArgs.Data);
            process.ErrorDataReceived += async (_, eventArgs) => await channelWriter.WriteAsync(eventArgs.Data);

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
                throw new StepFailedException {Name = step.Name};
        }
    }
}