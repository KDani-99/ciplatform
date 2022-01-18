using System.IO;
using CodeManager.Data.Configuration;
using CodeManager.Data.Entities.CI;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CodeManagerWebApi.Services
{
    public class ConfigParserService : IConfigParserService<RunConfiguration>
    {
        public RunConfiguration Parse(string data)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(LowerCaseNamingConvention.Instance)
                .Build();
            
            return deserializer.Deserialize<RunConfiguration>(new StreamReader(data));
        }
    }
}