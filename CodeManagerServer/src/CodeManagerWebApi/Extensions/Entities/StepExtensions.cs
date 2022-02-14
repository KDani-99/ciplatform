using CodeManager.Data.Entities;
using CodeManagerWebApi.DataTransfer;

namespace CodeManagerWebApi.Extensions.Entities
{
    public static class StepExtensions
    {
        public static StepDto ToDto(this Step step)
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