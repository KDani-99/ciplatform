using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeManager.Data.Repositories;
using CodeManagerAgentManager.Configuration;
using Microsoft.Extensions.Options;

namespace CodeManagerAgentManager.Services
{
    public sealed class LogStreamService : ILogStreamService
    {
        private readonly LogStreamServiceConfiguration _logStreamServiceConfiguration;
        private readonly IRunRepository _runRepository;
        
        public LogStreamService(IOptions<LogStreamServiceConfiguration> logStreamServiceConfiguration, IRunRepository runRepository)
        {
            _logStreamServiceConfiguration = logStreamServiceConfiguration.Value ?? throw new ArgumentNullException(nameof(logStreamServiceConfiguration));
            _runRepository = runRepository ?? throw new ArgumentNullException(nameof(runRepository));
        }
        
        public async Task WriteStreamAsync(IAsyncEnumerable<string> stream, long runId, long jobId, int stepIndex)
        {
            var numberOfLines = 0;
            var sizeInBytes = 0;

            var logPath = GetLogPath(runId, jobId, stepIndex);
            Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

            var run = await _runRepository.GetAsync(runId);
            var job = run.Jobs.First(item => item.Id == jobId); // TODO: lazy load!!
            var step = job.Steps[stepIndex];
            step.LogPath = logPath;

            await _runRepository.UpdateAsync(run);
            
            await using var outputStream = new StreamWriter(File.Open(logPath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read));
            
            await foreach (var line in stream)
            {
                if (_logStreamServiceConfiguration.MaxLinePerFile != 0 && numberOfLines >= _logStreamServiceConfiguration.MaxLinePerFile || sizeInBytes >= _logStreamServiceConfiguration.MaxFileSize)
                {
                    // TODO: warn log
                    break;
                }

                Console.Write(line);
                await outputStream.WriteAsync(line);
                await outputStream.FlushAsync();

                // TODO: forward stream

                sizeInBytes += Encoding.UTF8.GetByteCount(line);
                numberOfLines++;
            }
        }

        private string GetLogPath(long runId, long jobId, int step)
        {
            return Path.Join(_logStreamServiceConfiguration.LogPath, runId.ToString(), jobId.ToString(), $"{step}.log");
        }
    }
}