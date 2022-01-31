namespace CodeManagerAgent.Entities
{
    public class JobDetails
    {
        public string Repository { get; set; }
        public string Token { get; set; }
        public long RunId { get; set; }
        public long JobId { get; set; }
    }
}