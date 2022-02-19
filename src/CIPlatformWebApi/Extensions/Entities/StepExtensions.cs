using CIPlatform.Data.Entities;
using CIPlatformWebApi.DataTransfer;
using CIPlatformWebApi.DataTransfer.Step;

namespace CIPlatformWebApi.Extensions.Entities
{
    public static class StepExtensions
    {
        public static StepDto ToDto(this StepEntity step)
        {
            return new()
            {
                Id = step.Id,
                Name = step.Name,
                StartedDateTime = step.StartedDateTime,
                FinishedDateTime = step.FinishedDateTime,
                State = step.State
            };
        }
    }
}