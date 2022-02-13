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
    public class StepRepository : BaseRepository<Step>, IStepRepository
    {
        public StepRepository(CodeManagerDbContext dbContext) : base(dbContext)
        {
        }

        public override Task<Step> GetAsync(long id)
        {
            return DbContext
                   .Steps
                   .Include(x => x.Job)
                   .ThenInclude(x => x.Run)
                   .ThenInclude(x => x.Project)
                   .ThenInclude(x => x.Team)
                   .ThenInclude(x => x.Members)
                   .ThenInclude(x => x.User)
                   .OrderBy(x => x.Id)
                   .FirstOrDefaultAsync(step => step.Id == id);
        }

        public override Task<List<Step>> GetAsync(Expression<Func<Step, bool>> expression)
        {
            return DbContext.Steps.Where(expression).ToListAsync();
        }

        public override Task<bool> ExistsAsync(Expression<Func<Step, bool>> expression)
        {
            return DbContext.Steps.AnyAsync(expression);
        }

        public override async Task<long> CreateAsync(Step entity)
        {
            await DbContext.Steps.AddAsync(entity);
            await DbContext.SaveChangesAsync();

            return entity.Id;
        }

        public override async Task UpdateAsync(Step entity)
        {
            DbContext.Steps.Update(entity);
            await DbContext.SaveChangesAsync();
        }

        public override async Task DeleteAsync(long id)
        {
            var entity = await GetAsync(id);
            DbContext.Steps.Remove(entity);

            await DbContext.SaveChangesAsync();
        }
    }
}