using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CodeManager.Data.Database;
using CodeManager.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodeManager.Data.Repositories
{
    public class PlanRepository : BaseRepository<Plan> ,IPlanRepository
    {
        public PlanRepository(CodeManagerDbContext dbContext) : base(dbContext)
        {
        }
        
        public override Task<Plan> GetAsync(long id) => DbContext.Plans.FirstOrDefaultAsync(plan => plan.Id == id);

        public override Task<List<Plan>> GetAsync(Expression<Func<Plan, bool>> expression) =>
            DbContext.Plans.Where(expression).ToListAsync();

        public override Task<bool> ExistsAsync(Expression<Func<Plan, bool>> expression) => DbContext.Plans.AnyAsync(expression);

        public override async Task CreateAsync(Plan entity)
        {
            await DbContext.Plans.AddAsync(entity);
            await DbContext.SaveChangesAsync();
        }

        public override async Task UpdateAsync(Plan entity)
        {
            DbContext.Plans.Update(entity);
            await DbContext.SaveChangesAsync();
        }

        public override async Task DeleteAsync(long id)
        {
            var entity = await DbContext.Plans.Where(plan => plan.Id == id).FirstOrDefaultAsync();
            DbContext.Plans.Remove(entity);
            
            await DbContext.SaveChangesAsync();
        }
    }
}