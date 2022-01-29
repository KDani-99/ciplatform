namespace CodeManager.Core.Hubs.Common
{
    public class CommonAgentManagerHubMethods // TODO: the name of the class is where it receives those RPC calls (the client)
    {
        public const string QueueRunCommand = "QueueRunCommand";
        public const string RequestJobCommand = "RequestJobCommand";
        public const string StepResultEvent = "StepResultEvent";
    }
}