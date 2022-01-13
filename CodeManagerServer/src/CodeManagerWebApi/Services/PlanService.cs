using System;
using System.Threading.Tasks;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Entities;
using CodeManagerWebApi.Exceptions;
using CodeManagerWebApi.Repositories;

namespace CodeManagerWebApi.Services
{
    public class PlanService : IPlanService
    {
        private readonly IPlanRepository _planRepository;

        public PlanService(IPlanRepository planRepository)
        {
            _planRepository = planRepository ?? throw new ArgumentNullException(nameof(planRepository));
        }

        public async Task CreatePlanAsync(PlanDto planDto)
        {
            // TODO: Validate with attributes

            if (await _planRepository.ExistsAsync(planEntity => planEntity.Name == planDto.Name))
            {
                throw new PlanAlreadyExistsException();
            }
            
            var plan = new Plan
            {
                Name = planDto.Name,
                MaxCreatedTeamsPerUser = planDto.MaxCreatedTeamsPerUser,
                MaxJoinedTeamsPerUser = planDto.MaxJoinedTeamsPerUser
            };

            await _planRepository.CreateAsync(plan);
        }

        public async Task DeletePlanAsync(long id)
        {
            if (!await _planRepository.ExistsAsync(plan => plan.Id == id))
            {
                throw new PlanDoesNotExistException();
            }
            
            await _planRepository.DeleteAsync(id);
        }
    }
}