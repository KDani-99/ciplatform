using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CIPlatform.Data.Repositories
{
    public interface IRepository<T>
        where T : class
    {
        public Task<T> GetAsync(long id);
        public Task<List<T>> GetAsync(Expression<Func<T, bool>> expression);
        public Task<bool> ExistsAsync(Expression<Func<T, bool>> expression);
        public Task<long> CreateAsync(T entity);
        public Task UpdateAsync(T entity);
        public Task DeleteAsync(long id);
    }
}