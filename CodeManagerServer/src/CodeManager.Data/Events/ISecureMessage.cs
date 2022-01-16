namespace CodeManager.Data.Events
{
    public interface ISecureMessage
    {
        public string Token { get; set; }
    }
}