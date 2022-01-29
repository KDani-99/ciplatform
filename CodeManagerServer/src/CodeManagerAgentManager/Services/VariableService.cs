using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeManager.Core.Services;
using CodeManager.Data.Entities;
using CodeManager.Data.Repositories;

namespace CodeManagerAgentManager.Services
{
    public class VariableService : IVariableService
    {
        private readonly IVariableRepository _variableRepository;
        private readonly IEncryptionService _encryptionService;

        public VariableService(IVariableRepository variableRepository, IEncryptionService encryptionService)
        {
            _variableRepository = variableRepository ?? throw new ArgumentNullException(nameof(variableRepository));
            _encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
        }

        public async Task<IEnumerable<Variable>> GetVariablesForProject(long projectId)
        {
            var variables = await _variableRepository.GetAsync(var => var.Project.Id == projectId);
// TODO: add no tracking to repository
            foreach (var variable in variables.Where(variable => variable.IsSecret))
            {
                variable.Value = await _encryptionService.DecryptAsync(variable.Value);
            }

            return variables;
        }
    }
}