using System.Threading.Tasks;
using CodeManagerWebApi.Entities;

namespace CodeManagerWebApi.Repositories
{
    public interface IUserRepository : IRepository<User, long>
    {
        public Task<User> GetByUsernameAsync(string username);

        public Task<User> GetByEmailAsync(string email);
    }
}