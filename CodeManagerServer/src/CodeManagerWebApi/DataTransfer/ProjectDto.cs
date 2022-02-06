namespace CodeManagerWebApi.DataTransfer
{
    public class ProjectDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsPrivateProject { get; set; }
        public string TeamName { get; set; }
        public int Runs { get; set; }
        public string Owner { get; set; }
    }
}