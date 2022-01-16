using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManagerWebApi.Entities;

namespace CodeManagerWebApi.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        public Task<User> GetByUsernameAsync(string username);

        public Task<User> GetByEmailAsync(string email);
    }
}