using System.Threading.Tasks;

namespace CodeManager.Data.Services
{
    public interface IPlanService
    {
        public Task CreatePlanAsync(PlanDto planDto);
        public Task DeletePlanAsync(long id);
    }
}