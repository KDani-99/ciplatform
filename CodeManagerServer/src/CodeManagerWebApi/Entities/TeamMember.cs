namespace CodeManagerWebApi.Entities
{
    public class TeamMember
    {
        public User User { get; set; }
        public Permissions Permission { get; set; }
    }
}