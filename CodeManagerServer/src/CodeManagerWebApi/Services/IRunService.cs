﻿using System.IO;
using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManagerWebApi.DataTransfer;

namespace CodeManagerWebApi.Services
{
    public interface IRunService
    {
        public Task<RunDto> GetRunAsync(long runId);
        public Task<RunDataDto> GetRunAsync(long runId, User user);
        public Task<JobDataDto> GetJobAsync(long runId, long jobId, User user);
        public Task<Stream> GetStepFileStreamAsync(long runId, long jobId, long stepId, User user);
        public Task QueueRunAsync(long projectId, string instructions, User user);
    }
}