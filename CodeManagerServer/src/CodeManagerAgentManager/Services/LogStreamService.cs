using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using CodeManager.Data.Repositories;
using CodeManagerAgentManager.Configuration;
using CodeManagerAgentManager.Exceptions;
using CodeManagerAgentManager.WebSocket;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

namespace CodeManagerAgentManager.Services
{
    public sealed class LogStreamService : ILogStreamService
    {
        private readonly LogStreamServiceConfiguration _logStreamServiceConfiguration;
        private readonly IManagerClient _managerClient;
        private readonly IRunRepository _runRepository;

        public LogStreamService(IOptions<LogStreamServiceConfiguration> logStreamServiceConfiguration,
                                IRunRepository runRepository,
                                IManagerClient managerClient)
        {
            _logStreamServiceConfiguration = logStreamServiceConfiguration.Value ??
                throw new ArgumentNullException(nameof(logStreamServiceConfiguration));
            _runRepository = runRepository ?? throw new ArgumentNullException(nameof(runRepository));
            _managerClient = managerClient ?? throw new ArgumentNullException(nameof(managerClient));
        }

        public async Task ProcessStreamAsync(ChannelReader<string> stream, long runId, long jobId, int stepIndex)
        {
            var numberOfLines = 0;
            var sizeInBytes = 0;

            var logPath = Path.GetFullPath(GetLogPath(runId, jobId, stepIndex));
            Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

            var run = await _runRepository.GetAsync(runId);
            var job = run.Jobs.First(item => item.Id == jobId); // TODO: lazy load!!
            var step = job.Steps.First(x => x.Index == stepIndex);                    // TODO: use step id instead of index

            step.LogPath = logPath;
            await _runRepository.UpdateAsync(run);

            await using var outputStream =
                new StreamWriter(File.Open(logPath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read));

            var channel = Channel.CreateUnbounded<string>();
            await _managerClient.HubConnection.SendAsync("StreamLogToChannel", channel.Reader, runId, jobId, stepIndex);

            while (await stream.WaitToReadAsync())
            while (stream.TryRead(out var line))
            {
                if (_logStreamServiceConfiguration.MaxLinePerFile != 0 &&
                    numberOfLines >= _logStreamServiceConfiguration.MaxLinePerFile)
                    throw new LogStreamException(
                        $"Unable to write more data to file {logPath}. Log file has reached the maximum number of lines (Max.:{_logStreamServiceConfiguration.MaxLinePerFile}).");

                if (sizeInBytes >= _logStreamServiceConfiguration.MaxFileSize)
                    throw new LogStreamException(
                        $"Unable to write more data to file {logPath}. Log file has reached the maximum size (Max.:{_logStreamServiceConfiguration.MaxFileSize}).");

                await channel.Writer.WriteAsync(line); //TEST

                await outputStream.WriteAsync(line);
                await outputStream.FlushAsync();

                sizeInBytes += Encoding.UTF8.GetByteCount(line);
                numberOfLines++;
            }

            channel.Writer.Complete();
            
            outputStream.Close();
        }

        private string GetLogPath(long runId, long jobId, int step)
        {
            return Path.Join(_logStreamServiceConfiguration.LogPath, runId.ToString(), jobId.ToString(), $"{step}.log");
        }
    }
}