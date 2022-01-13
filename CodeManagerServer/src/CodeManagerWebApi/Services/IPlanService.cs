using System.Threading.Tasks;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Entities;

namespace CodeManagerWebApi.Services
{
    public interface IPlanService
    {
        public Task CreatePlanAsync(PlanDto planDto);
        public Task DeletePlanAsync(long id);
    }
}