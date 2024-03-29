﻿using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using CIPlatform.Data.Events;
using CIPlatform.Data.Repositories;
using CIPlatformManager.Configuration;
using CIPlatformManager.Exceptions;
using CIPlatformManager.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace CIPlatformManager.Services.Logs
{
    public sealed class LogStreamService : ILogStreamService
    {
        private readonly LogStreamServiceConfiguration _logStreamServiceConfiguration;
        private readonly IManagerClient _managerClient;
        private readonly IRunRepository _runRepository;
        private readonly IFileSystem _fileSystem;

        public LogStreamService(IOptions<LogStreamServiceConfiguration> logStreamServiceConfiguration,
                                IRunRepository runRepository,
                                IManagerClient managerClient,
                                IFileSystem fileSystem)
        {
            _logStreamServiceConfiguration = logStreamServiceConfiguration.Value ??
                throw new ArgumentNullException(nameof(logStreamServiceConfiguration));
            _runRepository = runRepository ?? throw new ArgumentNullException(nameof(runRepository));
            _managerClient = managerClient ?? throw new ArgumentNullException(nameof(managerClient));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public async Task ProcessStreamAsync(ChannelReader<string> stream, long runId, long jobId, int stepIndex)
        {
            var logPath = _fileSystem.Path.GetFullPath(GetLogPath(runId, jobId, stepIndex));
            _fileSystem.Directory.CreateDirectory(_fileSystem.Path.GetDirectoryName(logPath));

           // Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

            var run = await _runRepository.GetAsync(runId);
            var job = run.Jobs.First(item => item.Id == jobId);
            var step = job.Steps.First(item => item.Index == stepIndex);

            step.LogPath = logPath;
            await _runRepository.UpdateAsync(run);

            await using var outputStream =
                new StreamWriter(_fileSystem.File.Open(logPath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read));

            await StreamDataAsync(step.Id, logPath, outputStream, stream);
        }

        private async Task StreamDataAsync(long stepId, string logPath, StreamWriter outputStream, ChannelReader<string> stream)
        {
            var numberOfLines = 0;
            var sizeInBytes = 0;

            var channel = Channel.CreateUnbounded<string>();
            await _managerClient.HubConnection.SendAsync("StreamLogToChannel", channel.Reader, stepId);

            while (await stream.WaitToReadAsync())
            {
                while (stream.TryRead(out var line))
                {
                    CheckConstraints(numberOfLines, sizeInBytes, logPath);
                    
                    await channel.Writer.WriteAsync(line);

                    await outputStream.WriteAsync(line);
                    await outputStream.FlushAsync();

                    sizeInBytes += Encoding.UTF8.GetByteCount(line);
                    numberOfLines++;
                }
            }

            channel.Writer.Complete();
            
            outputStream.Close();
        }

        private void CheckConstraints(int numberOfLines, int sizeInBytes, string logPath)
        {
            if (_logStreamServiceConfiguration.MaxLinePerFile != 0 &&
                numberOfLines >= _logStreamServiceConfiguration.MaxLinePerFile)
                throw new LogStreamException(
                    $"Unable to write more data to file {logPath}. Log file has reached the maximum number of lines (Max.:{_logStreamServiceConfiguration.MaxLinePerFile}).");

            if (sizeInBytes >= _logStreamServiceConfiguration.MaxFileSize)
                throw new LogStreamException(
                    $"Unable to write more data to file {logPath}. Log file has reached the maximum size (Max.:{_logStreamServiceConfiguration.MaxFileSize}).");
        }

        private string GetLogPath(long runId, long jobId, int step)
        {
            return _fileSystem.Path.Join(_logStreamServiceConfiguration.LogPath, runId.ToString(), jobId.ToString(), $"{step}.log");
        }
    }
}