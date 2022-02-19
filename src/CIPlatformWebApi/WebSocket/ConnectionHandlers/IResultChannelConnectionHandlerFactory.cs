namespace CIPlatformWebApi.Strategies
{
    public interface IResultChannelConnectionHandlerFactory
    {
        public IResultChannelConnectionHandler Create(string type);
    }
}