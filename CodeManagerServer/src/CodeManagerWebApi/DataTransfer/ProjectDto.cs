namespace CodeManagerWebApi.DataTransfer
{
    public class ProjectDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string RepositoryUrl { get; set; }
        public bool IsPrivateRepository { get; set; }
        public bool IsPrivateProject { get; set; }
        public long TeamId { get; set; } // owner team id
    }
}