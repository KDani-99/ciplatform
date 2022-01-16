﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CodeManagerWebApi.Database;
using CodeManagerWebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodeManagerWebApi.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(CodeManagerDbContext dbContext) : base(dbContext)
        {
        }

        public Task<User> GetByUsernameAsync(string username)
        {
            return DbContext.Users.FirstOrDefaultAsync(user => user.Username == username);
        }

        public Task<User> GetByEmailAsync(string email)
        {
            return DbContext.Users.FirstOrDefaultAsync(user => user.Email == email);
        }

        public override Task<User> GetAsync(long id)
        {
            return DbContext.Users.FirstOrDefaultAsync(user => user.Id == id);
        }

        public override Task<List<User>> GetAsync(Expression<Func<User, bool>> expression)
        {
            return DbContext.Users.Where(expression).ToListAsync();
        }

        public override Task<bool> ExistsAsync(Expression<Func<User, bool>> expression)
        {
            return DbContext.Users.AnyAsync(expression);
        }

        public override async Task CreateAsync(User entity)
        {
            await DbContext.Users.AddAsync(entity);
            await DbContext.SaveChangesAsync();
        }

        public override async Task UpdateAsync(User entity)
        {
            DbContext.Users.Update(entity);
            await DbContext.SaveChangesAsync();
        }

        public override async Task DeleteAsync(long id)
        {
            var entity = await DbContext.Users.Where(user => user.Id == id).FirstOrDefaultAsync();
            DbContext.Users.Remove(entity);
            
            await DbContext.SaveChangesAsync();
        }
    }
}