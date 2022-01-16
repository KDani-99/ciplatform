using CodeManager.Data.Configuration;
using CodeManager.Data.Configuration.StartJob;

namespace CodeManager.Data.Messaging
{
    public class StartAgentJob
    {
        public JobConfiguration JobConfiguration { get; set; }
        public string Token { get; set; } // JWT Token with JobID, AgentID
    }
}