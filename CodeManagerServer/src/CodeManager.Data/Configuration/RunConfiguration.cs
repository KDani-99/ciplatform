using System.Collections.Generic;

namespace CodeManager.Data.Configuration
{
    public class RunConfiguration
    {
        public enum Triggers
        {
            Push,
            Merge
        }
        
        public string Name { get; set; }
        public Dictionary<Triggers, List<string>> Trigger { get; set; } // trigger: push: [<branch>, <branch1>]
        public Dictionary<string, JobConfiguration> Jobs { get; set; } // parallel execution?
    }
}