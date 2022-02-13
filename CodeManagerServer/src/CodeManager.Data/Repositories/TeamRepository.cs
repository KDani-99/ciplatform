﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CodeManager.Data.Database;
using CodeManager.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodeManager.Data.Repositories
{
    public class TeamRepository : BaseRepository<Team>, ITeamRepository
    {
        public TeamRepository(CodeManagerDbContext dbContext) : base(dbContext)
        {
        }

        public override Task<Team> GetAsync(long id)
        {
            return DbContext.Teams
                            .Include(team => team.Owner)
                            .Include(team => team.Projects)
                                .ThenInclude(project => project.Runs)
                            .Include(team => team.Members)
                                .ThenInclude(member => member.User)
                            .OrderBy(team => team.Id)
                            .FirstOrDefaultAsync(team => team.Id == id);
        }

        public override Task<List<Team>> GetAsync(Expression<Func<Team, bool>> expression)
        {
            return DbContext.Teams
                            .Include(team => team.Owner)
                            .Include(team => team.Projects)
                                .ThenInclude(project => project.Runs)
                            .Include(team => team.Members)
                                .ThenInclude(member => member.User)
                            .Where(expression)
                            .ToListAsync();
        }

        public override Task<bool> ExistsAsync(Expression<Func<Team, bool>> expression)
        {
            return DbContext.Teams.AnyAsync(expression);
        }

        public override async Task<long> CreateAsync(Team entity)
        {
            await DbContext.Teams.AddAsync(entity);
            await DbContext.SaveChangesAsync();

            return entity.Id;
        }

        public override async Task UpdateAsync(Team entity)
        {
            DbContext.Teams.Update(entity);
            await DbContext.SaveChangesAsync();
        }

        public override async Task DeleteAsync(long id)
        {
            var entity = await DbContext.Teams.Where(team => team.Id == id).FirstOrDefaultAsync();
            DbContext.Teams.Remove(entity);

            await DbContext.SaveChangesAsync();
        }
    }
}