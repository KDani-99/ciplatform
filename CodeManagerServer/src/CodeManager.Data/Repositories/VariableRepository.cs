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
    public class VariableRepository : BaseRepository<Variable>, IVariableRepository
    {
        public VariableRepository(CodeManagerDbContext dbContext) : base(dbContext)
        {
        }

        public override Task<Variable> GetAsync(long id)
        {
            return DbContext.Variables.FirstOrDefaultAsync(variable => variable.Id == id);
        }

        public override Task<List<Variable>> GetAsync(Expression<Func<Variable, bool>> expression)
        {
            return DbContext.Variables.Where(expression).ToListAsync();
        }

        public override Task<bool> ExistsAsync(Expression<Func<Variable, bool>> expression)
        {
            return DbContext.Variables.AnyAsync(expression);
        }

        public override async Task<long> CreateAsync(Variable entity)
        {
            await DbContext.Variables.AddAsync(entity);
            await DbContext.SaveChangesAsync();

            return entity.Id;
        }

        public override async Task UpdateAsync(Variable entity)
        {
            DbContext.Variables.Update(entity);
            await DbContext.SaveChangesAsync();
        }

        public override async Task DeleteAsync(long id)
        {
            var entity = await DbContext.Variables.Where(user => user.Id == id).FirstOrDefaultAsync();
            DbContext.Variables.Remove(entity);
            
            await DbContext.SaveChangesAsync();
        }
    }
}