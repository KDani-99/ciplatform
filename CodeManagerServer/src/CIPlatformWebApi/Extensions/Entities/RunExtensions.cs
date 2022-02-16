using System.Linq;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.DataTransfer;

namespace CIPlatformWebApi.Extensions.Entities
{
    public static class RunExtensions
    {
        public static RunDto ToDto(this Run run)
        {
            return new()
            {
                Id = run.Id,
                StartedDateTime = run.StartedDateTime,
                FinishedDateTime = run.FinishedDateTime,
                State = run.State,
                Jobs = run.Jobs.Count
            };
        }

        public static RunDataDto ToDataDto(this Run run)
        {
            return new()
            {
                Run = run.ToDto(),
                Jobs = run.Jobs.Select(job => job.ToDto())
            };
        }
    }
}