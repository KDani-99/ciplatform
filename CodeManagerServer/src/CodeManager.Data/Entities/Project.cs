﻿using System.Collections.Generic;
using CodeManager.Data.Entities.CI;

namespace CodeManager.Data.Entities
{
    public class Project : Entity
    {
        public string RepositoryUrl { get; set; }
        public bool IsPrivateRepository { get; set; }
        public bool IsPrivateProject { get; set; }
        public bool IsSSH { get; set; }
        public string SecretToken { get; set; } // Either SSH or access token
        public Team Team { get; set; }
        public List<Variable> Variables { get; set; }
        
        public List<Run> Runs { get; set; }
    }
}