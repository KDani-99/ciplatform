using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManagerWebApi.DataTransfer;

namespace CodeManagerWebApi.Services
{
    public interface IVariableService
    {
        public Task CreateOrUpdateVariableAsync(long projectId, VariableDto variableDto, User user);
    }
}