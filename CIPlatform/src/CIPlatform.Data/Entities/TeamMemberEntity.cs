using System;

namespace CIPlatform.Data.Entities
{
    public class TeamMember : Entity
    {
        public User User { get; set; }
        public TeamEntity Team { get; set; }
        public Permissions Permission { get; set; }
        public DateTime JoinTime { get; set; }
    }
}