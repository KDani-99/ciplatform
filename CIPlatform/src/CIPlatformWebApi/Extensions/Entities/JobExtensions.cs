using System.Linq;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.DataTransfer;
using CIPlatformWebApi.DataTransfer.Job;

namespace CIPlatformWebApi.Extensions.Entities
{
    public static class JobExtensions
    {
        public static JobDto ToDto(this JobEntity job)
        {
            return new()
            {
                Id = job.Id,
                State = job.State,
                StartedDateTime = job.StartedDateTime,
                FinishedDateTime = job.FinishedDateTime,
                Name = job.Name,
                JobContext = job.Context.ToString(),
                Steps = job.Steps.Count
            };
        }

        public static JobDataDto ToDataDto(this JobEntity job)
        {
            return new ()
            {
                Job = job.ToDto(),
                Steps = job.Steps.Select(step => step.ToDto())
            };
        }
    }
}