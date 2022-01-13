using System.Collections.Generic;
using CodeManagerShared.Configuration;

namespace CodeManagerShared.Entities
{
    public class Run
    {
        public enum Triggers
        {
            Push,
            Merge
        }
        
        public string Name { get; set; }
        public Dictionary<Triggers, List<string>> Trigger { get; set; } // trigger: push: [<branch>, <branch1>]
        public Dictionary<string, Job> Jobs { get; set; } // parallel execution?
    }
}