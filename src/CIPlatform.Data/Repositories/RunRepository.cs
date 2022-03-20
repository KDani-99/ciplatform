using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CIPlatform.Data.Database;
using CIPlatform.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CIPlatform.Data.Repositories
{
    public class RunRepository : BaseRepository<RunEntity>, IRunRepository
    {
        public RunRepository(CIPlatformDbContext dbContext) : base(dbContext)
        {
        }

        public override Task<RunEntity> GetAsync(long id)
        {
            return DbContext
                   .Runs
                   .Include(x => x.Jobs)
                   .ThenInclude(x => x.Steps)
                   .Include(x => x.Project)
                   .ThenInclude(x => x.Team)
                   .ThenInclude(x => x.Members)
                   .OrderBy(x => x.Id)
                   .FirstOrDefaultAsync(job => job.Id == id);
        }

        public override Task<List<RunEntity>> GetAsync(Expression<Func<RunEntity, bool>> expression)
        {
            return DbContext
                   .Runs
                   .Include(x => x.Jobs)
                   .ThenInclude(x => x.Steps)
                   .Include(x => x.Project)
                   .ThenInclude(x => x.Team)
                   .ThenInclude(x => x.Members)
                   .Where(expression).ToListAsync();
        }

        public override Task<bool> ExistsAsync(Expression<Func<RunEntity, bool>> expression)
        {
            return DbContext.Runs.AnyAsync(expression);
        }

        public override async Task<long> CreateAsync(RunEntity entity)
        {
            await DbContext.Runs.AddAsync(entity);
            await DbContext.SaveChangesAsync();

            return entity.Id;
        }

        public override async Task UpdateAsync(RunEntity entity)
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