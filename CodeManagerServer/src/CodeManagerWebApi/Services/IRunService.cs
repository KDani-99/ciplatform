using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManagerWebApi.DataTransfer;

namespace CodeManagerWebApi.Services
{
    public interface IRunService
    {
        public Task<RunDataDto> GetRunAsync(long runId, User user);
        public Task<JobDataDto> GetJobAsync(long runId, long jobId, User user);
        public Task<StepFileDto> GetStepFileAsync(long runId, long jobId, long stepId, User user);
        public Task QueueRunAsync(long projectId, string instructions, User user);
    }
}