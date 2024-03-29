﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CIPlatform.Data.Database;
using CIPlatform.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CIPlatform.Data.Repositories
{
    public class UserRepository : BaseRepository<UserEntity>, IUserRepository
    {
        public UserRepository(CIPlatformDbContext dbContext) : base(dbContext)
        {
        }

        public Task<UserEntity> GetByUsernameAsync(string username)
        {
            return DbContext.Users
                            .Include(user => user.Teams)
                            .OrderBy(user => user.Id)
                            .FirstOrDefaultAsync(user => user.Username == username);
        }

        public Task<UserEntity> GetByEmailAsync(string email)
        {
            return DbContext.Users
                            .Include(user => user.Teams)
                            .OrderBy(user => user.Id)
                            .FirstOrDefaultAsync(user => user.Email == email);
        }

        public override Task<UserEntity> GetAsync(long id)
        {
            return DbContext.Users
                            .Include(user => user.Teams)
                            .OrderBy(user => user.Id)
                            .FirstOrDefaultAsync(user => user.Id == id);
        }

        public override Task<List<UserEntity>> GetAsync(Expression<Func<UserEntity, bool>> expression)
        {
            return DbContext.Users
                            .Include(user => user.Teams).Where(expression).ToListAsync();
        }

        public override Task<bool> ExistsAsync(Expression<Func<UserEntity, bool>> expression)
        {
            return DbContext.Users.AnyAsync(expression);
        }

        public override async Task<long> CreateAsync(UserEntity entity)
        {
            await DbContext.Users.AddAsync(entity);
            await DbContext.SaveChangesAsync();

            return entity.Id;
        }

        public override async Task UpdateAsync(UserEntity entity)
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