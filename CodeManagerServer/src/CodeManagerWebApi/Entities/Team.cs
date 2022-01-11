using System.Collections;
using System.Collections.Generic;

namespace CodeManagerWebApi.Entities
{
    public class Team : Entity
    {
        public User Owner { get; set; }
        public string Name { get; set; }
        public List<TeamMember> Members { get; set; }
        public string Image { get; set; }
        public IEnumerable<string> ProjectIds { get; set; }
    }
}