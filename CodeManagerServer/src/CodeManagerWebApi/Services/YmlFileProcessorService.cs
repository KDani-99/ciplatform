using System;
using System.Text;
using System.Threading.Tasks;
using CodeManager.Data.Configuration;
using CodeManager.Data.Repositories;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CodeManagerWebApi.Services
{
    public class YmlFileProcessorService : IFileProcessorService<RunConfiguration>
    {
        private readonly IDeserializer _deserializer;

        public YmlFileProcessorService(IDeserializer deserializer)
        {
            _deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
        }
        
        public Task<RunConfiguration> ProcessAsync(string data, long projectId)
        {
            return Task.FromResult(_deserializer.Deserialize<RunConfiguration>(data));
        }
    }
}