using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CIPlatform.Data.Database;
using CIPlatform.Data.Entities;

namespace CIPlatform.Data.Repositories
{
    public abstract class BaseRepository<T> : IRepository<T>
        where T : Entity
    {
        protected readonly CIPlatformDbContext DbContext;

        protected BaseRepository(CIPlatformDbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public abstract Task<T> GetAsync(long id);

        public abstract Task<List<T>> GetAsync(Expression<Func<T, bool>> expression);

        public abstract Task<bool> ExistsAsync(Expression<Func<T, bool>> expression);

        public abstract Task<long> CreateAsync(T entity);

        public abstract Task UpdateAsync(T entity);
        public abstract Task DeleteAsync(long id);
    }
}