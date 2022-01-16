namespace CodeManager.Data.Entities
{
    public class TeamMember : Entity
    {
        public User User { get; set; }
        public Team Team { get; set; }
        public Permissions Permission { get; set; }
    }
}