using System;
using System.Text;
using System.Threading.Tasks;
using CodeManager.Data.Configuration;
using CodeManager.Data.Repositories;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CodeManagerAgentManager.Services
{
    public class YmlFileProcessorService : IFileProcessorService<RunConfiguration>
    {
        private readonly IDeserializer _deserializer;
        private readonly IVariableRepository _variableRepository;
        
        public YmlFileProcessorService(IDeserializer deserializer, IVariableRepository variableRepository)
        {
            _deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
            _variableRepository = variableRepository ?? throw new ArgumentNullException(nameof(variableRepository));
        }
        
        public async Task<RunConfiguration> ProcessAsync(string data, long projectId)
        {
            // TODO: validate max steps, etc.
            var runConfiguration = _deserializer.Deserialize<RunConfiguration>(data);
            var variables = await _variableRepository.GetAsync(p => p.Project.Id == projectId);
            
            // TODO: add new method
            foreach (var job in runConfiguration.Jobs)
            {
                foreach (var step in job.Value.Steps)
                {
                    var sb = new StringBuilder(step.Cmd);

                    foreach (var variable in variables)
                    {
                        // format: $(VariableName)
                        sb.Replace($"$({variable.Name})", variable.Value);
                    }

                    step.Cmd = sb.ToString();
                }
            }
            
            return runConfiguration;
        }
    }
}