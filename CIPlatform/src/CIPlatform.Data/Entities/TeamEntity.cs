using System.Collections.Generic;

namespace CIPlatform.Data.Entities
{
    public class TeamEntity : Entity
    {
        public UserEntity Owner { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public string Image { get; set; }
        public List<TeamMemberEntity> Members { get; set; } = new();
        public List<ProjectEntity> Projects { get; set; } = new();
    }
}