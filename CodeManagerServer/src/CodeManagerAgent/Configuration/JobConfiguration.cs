using Docker.DotNet.Models;

namespace CodeManagerAgent.Configuration
{
    public class JobConfiguration
    {
        public AuthConfig AuthConfig { get; set; }
        public string ConfigPath { get; set; }
        public string LogPath { get; set; }
        public string AgentId { get; set; }
    }
}