using System.Threading.Tasks;
using CIPlatform.Data.Entities;

namespace CIPlatform.Data.Repositories
{
    public interface IUserRepository : IRepository<UserEntity>
    {
        public Task<UserEntity> GetByUsernameAsync(string username);

        public Task<UserEntity> GetByEmailAsync(string email);
    }
}