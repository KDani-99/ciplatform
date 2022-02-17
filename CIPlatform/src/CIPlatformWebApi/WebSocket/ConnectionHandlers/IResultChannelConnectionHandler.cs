using System.Threading.Tasks;
using CIPlatform.Data.Entities;

namespace CIPlatformWebApi.Strategies
{
    public interface IResultChannelConnectionHandler
    {
        public Task<bool> VerifyAsync(long entityId, User user);
    }
}