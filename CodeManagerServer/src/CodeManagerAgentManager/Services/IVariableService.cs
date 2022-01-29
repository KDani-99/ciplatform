using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeManager.Data.Entities;

namespace CodeManagerAgentManager.Services
{
    public interface IVariableService
    {
        public Task<IEnumerable<Variable>> GetVariablesForProject(long projectId);
    }
}