using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CodeManagerWebApi.Entities;
using MongoDB.Driver;

namespace CodeManagerWebApi.Repositories
{
    public interface IRepository<T, TId>
        where T : class
    {
        public Task<T> GetAsync(TId id);
        public Task<List<T>> GetAsync(Expression<Func<T, bool>> expression);
        public Task<bool> ExistsAsync(TId id);
        public Task CreateAsync(T entity);
        public Task UpdateAsync(T entity);
    }
}