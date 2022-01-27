using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CodeManager.Data.Database;
using CodeManager.Data.Entities;

namespace CodeManager.Data.Repositories
{
    public class ProjectRepository : BaseRepository<Project>, IProjectRepository
    {
        public ProjectRepository(CodeManagerDbContext dbContext) : base(dbContext)
        {
        }

        public override Task<Project> GetAsync(long id)
        {
            throw new NotImplementedException();
        }

        public override Task<List<Project>> GetAsync(Expression<Func<Project, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> ExistsAsync(Expression<Func<Project, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public override Task<long> CreateAsync(Project entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateAsync(Project entity)
        {
            throw new NotImplementedException();
        }

        public override Task DeleteAsync(long id)
        {
            throw new NotImplementedException();
        }
    }
}