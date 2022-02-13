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
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(CodeManagerDbContext dbContext) : base(dbContext)
        {
        }

        public Task<User> GetByUsernameAsync(string username)
        {
            return DbContext.Users
                            .Include(user => user.Teams)
                            .OrderBy(user => user.Id)
                            .FirstOrDefaultAsync(user => user.Username == username);
        }

        public Task<User> GetByEmailAsync(string email)
        {
            return DbContext.Users
                            .Include(user => user.Teams)
                            .OrderBy(user => user.Id)
                            .FirstOrDefaultAsync(user => user.Email == email);
        }

        public override Task<User> GetAsync(long id)
        {
            return DbContext.Users
                            .Include(user => user.Teams)
                            .OrderBy(user => user.Id)
                            .FirstOrDefaultAsync(user => user.Id == id);
        }

        public override Task<List<User>> GetAsync(Expression<Func<User, bool>> expression)
        {
            return DbContext.Users
                            .Include(user => user.Teams).Where(expression).ToListAsync();
        }

        public override Task<bool> ExistsAsync(Expression<Func<User, bool>> expression)
        {
            return DbContext.Users.AnyAsync(expression);
        }

        public override async Task<long> CreateAsync(User entity)
        {
            await DbContext.Users.AddAsync(entity);
            await DbContext.SaveChangesAsync();

            return entity.Id;
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