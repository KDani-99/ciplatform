namespace CodeManager.Data.Models
{
    public class Plan : Entity
    {
        public string Name { get; set; }
        public int MaxCreatedTeamsPerUser { get; set; }
        public int MaxJoinedTeamsPerUser { get; set; }
    }
}