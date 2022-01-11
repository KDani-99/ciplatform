using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CodeManagerWebApi.Database;
using CodeManagerWebApi.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace CodeManagerWebApi.Repositories
{
    public class TeamRepository : BaseRepository<Team, long>, ITeamRepository
    {
        public TeamRepository(CodeManagerDbContext dbContext) : base(dbContext)
        {
        }

        public override Task<Team> GetAsync(long id) => DbContext.Teams.FirstOrDefaultAsync(team => team.Id == id);

        public override Task<List<Team>> GetAsync(Expression<Func<Team, bool>> expression) =>
            DbContext.Teams.Where(expression).ToListAsync();

        public override Task<bool> ExistsAsync(long id) => DbContext.Teams.AnyAsync(user => user.Id == id);

        public override async Task CreateAsync(Team entity)
        {
            await DbContext.Teams.AddAsync(entity);
            await DbContext.SaveChangesAsync();
        }

        public override async Task UpdateAsync(Team entity)
        {
            DbContext.Teams.Update(entity);
            await DbContext.SaveChangesAsync();
        }
    }
}