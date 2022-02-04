using System;
using System.Linq;
using System.Threading.Tasks;
using CodeManager.Core.Services;
using CodeManager.Data.Entities;
using CodeManager.Data.Repositories;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Exceptions;

namespace CodeManagerWebApi.Services
{
    public class VariableService : IVariableService
    {
        private readonly IVariableRepository _variableRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IEncryptionService _encryptionService;

        public VariableService(IVariableRepository variableRepository, IProjectRepository projectRepository, IEncryptionService encryptionService)
        {
            _variableRepository = variableRepository ?? throw new ArgumentNullException(nameof(variableRepository));
            _encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        }
        
        public async Task CreateOrUpdateVariableAsync(ProjectDto projectDto, VariableDto variableDto)
        {
            var project = await _projectRepository.GetAsync(projectDto.Id);

            if (project == default)
            {
                throw new ProjectDoesNotExistException();
            }
            
            var variable = project.Variables.FirstOrDefault(var => var.Name == variableDto.Name);
            var value = variableDto.Value;
                
            /*if (variableDto.IsSecret)
            {
                value = await _encryptionService.EncryptAsync(value);
            }*/

            if (variable == default)
            {
                await _variableRepository.CreateAsync(new Variable
                {
                    Name = variableDto.Name,
                    Value = value,
                    IsSecret = variableDto.IsSecret
                });
            }
            else
            {
                variable.Name = variableDto.Name;
                variable.Value = value;
                variable.IsSecret = variableDto.IsSecret;

                await _variableRepository.UpdateAsync(variable);
            }
        }
    }
}