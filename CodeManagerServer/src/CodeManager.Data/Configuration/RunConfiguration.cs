using System.Collections.Generic;

namespace CodeManager.Data.Configuration
{
    public class RunConfiguration
    {
        public string Name { get; set; }
        public Dictionary<string, JobConfiguration> Jobs { get; set; }  // parallel execution?
    }
}