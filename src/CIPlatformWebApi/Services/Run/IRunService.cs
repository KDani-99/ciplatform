using System.IO;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.DataTransfer;
using CIPlatformWebApi.DataTransfer.Job;
using CIPlatformWebApi.DataTransfer.Run;

namespace CIPlatformWebApi.Services.Run
{
    public interface IRunService
    {
        public Task<RunDto> GetRunAsync(long runId);
        public Task<RunDto> GetRunAsync(long runId,  UserEntity user);
        public Task<RunDataDto> GetRunDataAsync(long runId, UserEntity user);
        public Task<JobDataDto> GetJobAsync(long runId, long jobId, UserEntity user);
        public Task<Stream> GetStepFileStreamAsync(long runId, long jobId, long stepId, UserEntity user);
        public Task<RunDto> CreateRunAsync(long projectId, string instructions, UserEntity user);
        public Task DeleteRunAsync(long runId, UserEntity user);
        public Task<bool> IsAllowedRun(long runId, UserEntity user);
        public Task<bool> IsAllowedJob(long jobId, UserEntity user);
        public Task<bool> IsAllowedStep(long stepId, UserEntity user);
    }
}