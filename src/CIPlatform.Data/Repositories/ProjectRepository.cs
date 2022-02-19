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
    public class ProjectRepository : BaseRepository<ProjectEntity>, IProjectRepository
    {
        public ProjectRepository(CIPlatformDbContext dbContext) : base(dbContext)
        {
        }

        public override Task<ProjectEntity> GetAsync(long id)
        {
            return DbContext
                   .Projects
                   .Include(x => x.Runs)
                   .ThenInclude(x => x.Jobs)
                   .Include(x => x.Team)
                   .ThenInclude(x => x.Members)
                   .ThenInclude(x => x.User)
                   .OrderBy(x => x.Id)
                   .FirstOrDefaultAsync(x => x.Id == id);
        }

        public override Task<List<ProjectEntity>> GetAsync(Expression<Func<ProjectEntity, bool>> expression)
        {
            return DbContext
                   .Projects
                   .Include(x => x.Runs)
                   .ThenInclude(x => x.Jobs)
                   .Include(x => x.Team)
                   .ThenInclude(x => x.Members)
                   .Where(expression)
                   .ToListAsync();
        }

        public override Task<bool> ExistsAsync(Expression<Func<ProjectEntity, bool>> expression)
        {
            return DbContext.Projects.AnyAsync(expression);
        }

        public override async Task<long> CreateAsync(ProjectEntity entity)
        {
            await DbContext.Projects.AddAsync(entity);
            await DbContext.SaveChangesAsync();

            return entity.Id;
        }

        public override async Task UpdateAsync(ProjectEntity entity)
        {
            DbContext.Projects.Update(entity);
            await DbContext.SaveChangesAsync();
        }

        public override async Task DeleteAsync(long id)
        {
            var entity = await DbContext.Projects.Where(project => project.Id == id).FirstOrDefaultAsync();
            DbContext.Projects.Remove(entity);

            await DbContext.SaveChangesAsync();
        }
    }
}