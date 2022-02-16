using System.Linq;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.DataTransfer;

namespace CIPlatformWebApi.Extensions.Entities
{
    public static class JobExtensions
    {
        public static JobDto ToDto(this Job job) // TODO: => use extension as it is not related and the entity itself should not know about the extensions
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

        public static JobDataDto ToDataDto(this Job job)
        {
            return new ()
            {
                Job = job.ToDto(),
                Steps = job.Steps.Select(step => step.ToDto())
            };
        }
    }
}