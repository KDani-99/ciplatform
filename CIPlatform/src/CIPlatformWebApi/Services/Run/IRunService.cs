﻿using System.IO;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.DataTransfer;
using CIPlatformWebApi.DataTransfer.Job;
using CIPlatformWebApi.DataTransfer.Run;

namespace CIPlatformWebApi.Services
{
    public interface IRunService
    {
        public Task<RunDto> GetRunAsync(long runId);
        public Task<RunDataDto> GetRunAsync(long runId, User user);
        public Task<JobDataDto> GetJobAsync(long runId, long jobId, User user);
        public Task<Stream> GetStepFileStreamAsync(long runId, long jobId, long stepId, User user);
        public Task QueueRunAsync(long projectId, string instructions, User user);
        public Task DeleteRunAsync(long runId, User user);
        public Task<bool> IsAllowedRun(long runId, User user);
        public Task<bool> IsAllowedJob(long jobId, User user);
        public Task<bool> IsAllowedStep(long stepId, User user);
    }
}