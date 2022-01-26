using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CodeManager.Data.Database;
using CodeManager.Data.Entities;

namespace CodeManager.Data.Repositories
{
    public class VariableRepository : BaseRepository<Variable>, IVariableRepository
    {
        public VariableRepository(CodeManagerDbContext dbContext) : base(dbContext)
        {
        }

        public override Task<Variable> GetAsync(long id)
        {
            throw new NotImplementedException();
        }

        public override Task<List<Variable>> GetAsync(Expression<Func<Variable, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> ExistsAsync(Expression<Func<Variable, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public override Task<long> CreateAsync(Variable entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateAsync(Variable entity)
        {
            throw new NotImplementedException();
        }

        public override Task DeleteAsync(long id)
        {
            throw new NotImplementedException();
        }
    }
}