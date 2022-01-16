using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CodeManagerWebApi.Entities;

namespace CodeManagerWebApi.Repositories
{
    public interface IRepository<T>
        where T : class
    {
        public Task<T> GetAsync(long id);
        public Task<List<T>> GetAsync(Expression<Func<T, bool>> expression);
        public Task<bool> ExistsAsync(Expression<Func<T, bool>> expression);
        public Task CreateAsync(T entity);
        public Task UpdateAsync(T entity);
        public Task DeleteAsync(long id);
    }
}