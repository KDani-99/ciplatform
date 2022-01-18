using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CodeManager.Data.Database;
using CodeManager.Data.Entities.CI;
using Microsoft.EntityFrameworkCore;

namespace CodeManager.Data.Repositories
{
    public class RunRepository : BaseRepository<Run>, IRunRepository
    {
        public RunRepository(CodeManagerDbContext dbContext) : base(dbContext)
        {
        }

        public override Task<Run> GetAsync(long id)
        {
            return DbContext.Runs.FirstOrDefaultAsync(job => job.Id == id);
        }

        public override Task<List<Run>> GetAsync(Expression<Func<Run, bool>> expression)
        {
            return DbContext.Runs.Where(expression).ToListAsync();
        }

        public override Task<bool> ExistsAsync(Expression<Func<Run, bool>> expression)
        {
            return DbContext.Runs.AnyAsync(expression);
        }

        public override async Task<long> CreateAsync(Run entity)
        {
            await DbContext.Runs.AddAsync(entity);
            await DbContext.SaveChangesAsync();

            return entity.Id;
        }

        public override async Task UpdateAsync(Run entity)
        {
            DbContext.Runs.Update(entity);
            await DbContext.SaveChangesAsync();
        }

        public override async Task DeleteAsync(long id)
        {
            var entity = await GetAsync(id);
            DbContext.Runs.Remove(entity);
            
            await DbContext.SaveChangesAsync();
        }
    }
}