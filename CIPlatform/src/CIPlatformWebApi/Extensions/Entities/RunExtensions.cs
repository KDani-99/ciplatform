using System.Linq;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.DataTransfer;
using CIPlatformWebApi.DataTransfer.Run;

namespace CIPlatformWebApi.Extensions.Entities
{
    public static class RunExtensions
    {
        public static RunDto ToDto(this RunEntity run)
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

        public static RunDataDto ToDataDto(this RunEntity run)
        {
            return new()
            {
                Run = run.ToDto(),
                Jobs = run.Jobs.Select(job => job.ToDto())
            };
        }
    }
}