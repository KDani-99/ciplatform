using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CodeManagerAgentManager.Configuration;
using Microsoft.Extensions.Options;

namespace CodeManagerAgentManager.Services
{
    public sealed class LogStreamService : ILogStreamService
    {
        private readonly LogStreamServiceConfiguration _logStreamServiceConfiguration;
        
        public LogStreamService(IOptions<LogStreamServiceConfiguration> logStreamServiceConfiguration)
        {
            _logStreamServiceConfiguration = logStreamServiceConfiguration.Value ?? throw new ArgumentNullException(nameof(logStreamServiceConfiguration));
        }
        
        public async Task WriteStreamAsync(int runId, int jobId, int step, IAsyncEnumerable<string> stream)
        {
            var numberOfLines = 0;
            var sizeInBytes = 0;
            
            await using var outputStream = new StreamWriter(File.Open(GetLogPath(runId, jobId, step), FileMode.Create, FileAccess.ReadWrite, FileShare.Read));
            
            await foreach (var line in stream)
            {
                if (numberOfLines >= _logStreamServiceConfiguration.MaxLinePerFile || sizeInBytes >= _logStreamServiceConfiguration.MaxFileSize)
                {
                    // TODO: warn log
                    break;
                }

                await outputStream.WriteAsync(line);
                await outputStream.FlushAsync();

                sizeInBytes += Encoding.UTF8.GetByteCount(line);
                numberOfLines++;
            }
        }

        private string GetLogPath(int runId, int jobId, int step)
        {
            return Path.Join(_logStreamServiceConfiguration.LogPath, runId.ToString(), jobId.ToString(), $"{step}.log");
        }
    }
}