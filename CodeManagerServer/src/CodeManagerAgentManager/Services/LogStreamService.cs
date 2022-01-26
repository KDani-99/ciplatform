using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManager.Data.Repositories;
using CodeManagerAgentManager.Configuration;
using CodeManagerAgentManager.Exceptions;
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

            var secrets = run.Project.Variables
                .Where(variable => variable.IsSecret)
                .Select(s => s.Value)
                .ToList();
            
            step.LogPath = logPath;
            await _runRepository.UpdateAsync(run);
            
            await using var outputStream = new StreamWriter(File.Open(logPath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read));
            
            await foreach (var line in stream)
            {
                if (_logStreamServiceConfiguration.MaxLinePerFile != 0 && numberOfLines >= _logStreamServiceConfiguration.MaxLinePerFile)
                {
                    throw new LogStreamException($"Unable to write more data to file {logPath}. Log file has reached the maximum number of lines (Max.:{_logStreamServiceConfiguration.MaxLinePerFile}).");
                }

                if (sizeInBytes >= _logStreamServiceConfiguration.MaxFileSize)
                {
                    throw new LogStreamException($"Unable to write more data to file {logPath}. Log file has reached the maximum size (Max.:{_logStreamServiceConfiguration.MaxFileSize}).");
                }

                var filtered = FilterSecrets(line, secrets);

                Console.Write(filtered);
                await outputStream.WriteAsync(filtered);
                await outputStream.FlushAsync();

                // TODO: forward stream

                sizeInBytes += Encoding.UTF8.GetByteCount(filtered);
                numberOfLines++;
            }
        }

        private string GetLogPath(long runId, long jobId, int step)
        {
            return Path.Join(_logStreamServiceConfiguration.LogPath, runId.ToString(), jobId.ToString(), $"{step}.log");
        }

        private static string FilterSecrets(string log, IEnumerable<string> secrets)
        {
            // the reason why it needs to be filtered here is because a command might output a secret,
            // even if it gets marked before sending it to the agent
            // (so it would have been faster to stream it back as it would not require additional processing)
            var sb = new StringBuilder(log);

            foreach (var secret in secrets)
            {
                sb.Replace(secret, "********");
            }
            
            return sb.ToString();
        }
    }
}