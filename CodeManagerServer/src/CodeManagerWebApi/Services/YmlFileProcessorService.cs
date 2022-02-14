using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeManager.Data.Configuration;
using CodeManagerWebApi.Configuration;
using CodeManagerWebApi.Exceptions;
using Microsoft.Extensions.Options;
using YamlDotNet.Serialization;

namespace CodeManagerWebApi.Services
{
    public class YmlFileProcessorService : IFileProcessorService<RunConfiguration>
    {
        private readonly IDeserializer _deserializer;
        private readonly YmlConfiguration _ymlConfiguration;

        public YmlFileProcessorService(IDeserializer deserializer, IOptions<YmlConfiguration> ymlConfiguration)
        {
            _deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
            _ymlConfiguration = ymlConfiguration.Value ?? throw new ArgumentNullException(nameof(ymlConfiguration));
        }

        public Task<RunConfiguration> ProcessAsync(string data, long projectId)
        {
            var result = _deserializer.Deserialize<RunConfiguration>(data);

            ValidateConfiguration(result);

            return Task.FromResult(result);
        }

        private void ValidateConfiguration(RunConfiguration configuration)
        {
            if (configuration.Jobs?.Count > _ymlConfiguration.MaxJobCount)
            {
                throw new InvalidInstructionFileException(
                    "The configuration file exceeds the maximum number of allowed jobs.");
            }

            ValidateJobConfigurations(configuration.Jobs);
        }

        private void ValidateJobConfigurations(Dictionary<string, JobConfiguration> jobConfigurations)
        {
            foreach (var (key, value) in jobConfigurations)
            {
                if (value.Steps.Count > _ymlConfiguration.MaxStepPerJobCount)
                {
                    throw new InvalidInstructionFileException(
                        "The configuration file exceeds the maximum number of steps per job.");
                }

                if (key.Length is > 25 or < 2)
                {
                    throw new InvalidInstructionFileException("Job name must be between 1 and 25 characters.");
                }

                ValidateStepConfigurations(value.Steps);
            }
        }

        private static void ValidateStepConfigurations(IEnumerable<StepConfiguration> stepConfigurations)
        {
            foreach (var stepConfig in stepConfigurations)
            {
                if (stepConfig.Name.Length is > 50 or < 1)
                {
                    throw new InvalidInstructionFileException("Step name must be between 1 and 50 characters");
                }

                if (stepConfig.Cmd.Length > 999 || stepConfig.Cmd.Length < 1)
                {
                    throw new InvalidInstructionFileException("Step command must be between 1 and 999 characters");
                }
            }
        }
    }
}