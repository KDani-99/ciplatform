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
    public class ProjectRepository : BaseRepository<Project>, IProjectRepository
    {
        public ProjectRepository(CodeManagerDbContext dbContext) : base(dbContext)
        {
        }

        public override Task<Project> GetAsync(long id)
        {
            return DbContext
                .Projects
                .Include(x => x.Variables)
                .Include(x => x.Team)
                .ThenInclude(x => x.Members)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(project => project.Id == id);
        }

        public override Task<List<Project>> GetAsync(Expression<Func<Project, bool>> expression)
        {
            return DbContext
                .Projects
                .Include(x => x.Variables)
                .Include(x => x.Team)
                .ThenInclude(x => x.Members)
                .Where(expression)
                .ToListAsync();
        }

        public override Task<bool> ExistsAsync(Expression<Func<Project, bool>> expression)
        {
            return DbContext.Projects.AnyAsync(expression);
        }

        public override async Task<long> CreateAsync(Project entity)
        {
            await DbContext.Projects.AddAsync(entity);
            await DbContext.SaveChangesAsync();

            return entity.Id;
        }

        public override async Task UpdateAsync(Project entity)
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