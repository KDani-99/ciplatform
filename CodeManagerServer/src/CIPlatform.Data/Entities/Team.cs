using System.Collections.Generic;

namespace CIPlatform.Data.Entities
{
    public class Team : Entity
    {
        public User Owner { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public string Image { get; set; }
        public List<TeamMember> Members { get; set; } = new();
        public List<Project> Projects { get; set; } = new();
    }
}