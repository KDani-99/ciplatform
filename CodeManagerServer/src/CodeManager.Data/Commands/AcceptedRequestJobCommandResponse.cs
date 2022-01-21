using CodeManager.Data.Configuration;

namespace CodeManager.Data.Commands
{
    public class AcceptedRequestJobCommandResponse
    {
        public string Token { get; init; }
        public JobConfiguration JobConfiguration { get; init; }
    }
}