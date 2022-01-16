﻿using System.Collections;
using System.Collections.Generic;

namespace CodeManager.Data.Entities
{
    public class Team : Entity
    {
        public User Owner { get; set; }
        public string Name { get; set; }
        public List<TeamMember> Members { get; set; }
        public string Image { get; set; }
        public List<Project> Projects { get; set; }
    }
}