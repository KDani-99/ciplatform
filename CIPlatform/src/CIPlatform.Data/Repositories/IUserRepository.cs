using System.Threading.Tasks;
using CIPlatform.Data.Entities;

namespace CIPlatform.Data.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        public Task<User> GetByUsernameAsync(string username);

        public Task<User> GetByEmailAsync(string email);
    }
}