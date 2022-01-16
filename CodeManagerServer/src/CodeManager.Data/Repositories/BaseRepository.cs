using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CodeManager.Data.Database;
using CodeManager.Data.Entities;

namespace CodeManager.Data.Repositories
{
    public abstract class BaseRepository<T> : IRepository<T>
        where T : Entity
    {

        protected readonly CodeManagerDbContext DbContext;

        protected BaseRepository(CodeManagerDbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        public abstract Task<T> GetAsync(long id);

       public abstract Task<List<T>> GetAsync(Expression<Func<T, bool>> expression);

       public abstract Task<bool> ExistsAsync(Expression<Func<T, bool>> expression);

       public abstract Task CreateAsync(T entity);

       public abstract Task UpdateAsync(T entity);
       public abstract Task DeleteAsync(long id);
    }
}