namespace CodeManagerWebApi.Entities
{
    public class Project : Entity
    {
        public string RepositoryUrl { get; set; }
        public bool IsPrivateRepository { get; set; }
        public bool IsPrivateProject { get; set; }
        public bool IsSSH { get; set; }
        public string SecretToken { get; set; } // Either SSH or access token
        
        public Team Team { get; set; }
    }
}