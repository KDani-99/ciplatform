using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CodeManagerWebApi.Database;
using CodeManagerWebApi.Entities;
using MongoDB.Driver;

namespace CodeManagerWebApi.Repositories
{
    public abstract class BaseRepository<T, TId> : IRepository<T, TId>
        where T : Entity
    {

        protected readonly CodeManagerDbContext DbContext;

        protected BaseRepository(CodeManagerDbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        
       /* public Task<T> GetAsync(string id) => DbContext.(entity => entity.Id == id)();
        public async Task<bool> ExistsAsync(string id) => await GetAsync(id) is not null;

        public Task CreateAsync(T entity) => EntityCollection.InsertOneAsync(entity);
        
        public Task UpdateAsync(T entity) => EntityCollection.ReplaceOneAsync(document => document.Id == entity.Id, entity);

        public Task<List<T>> GetAsync(FilterDefinition<T> filter) => EntityCollection.Find(filter).ToListAsync();*/
       public abstract Task<T> GetAsync(TId id);

       public abstract Task<List<T>> GetAsync(Expression<Func<T, bool>> expression);

       public abstract Task<bool> ExistsAsync(TId id);

       public abstract Task CreateAsync(T entity);

       public abstract Task UpdateAsync(T entity);
    }
}