using System.Threading.Tasks;
using CodeManagerWebApi.DataTransfer;

namespace CodeManagerWebApi.Services
{
    public interface IJobService
    {
        public Task<StepFileDto> GetStepFileAsync(long stepId);
    }
}