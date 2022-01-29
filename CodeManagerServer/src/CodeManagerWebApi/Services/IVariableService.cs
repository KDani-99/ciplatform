using System.Threading.Tasks;
using CodeManager.Data.DataTransfer;
using CodeManager.Data.Entities;
using CodeManagerWebApi.DataTransfer;

namespace CodeManagerWebApi.Services
{
    public interface IVariableService
    {
        public Task CreateOrUpdateVariableAsync(Project project,VariableDto variableDto);
    }
}