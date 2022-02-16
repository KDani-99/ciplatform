using System.Collections.Generic;

namespace CIPlatform.Data.Configuration
{
    public class RunConfiguration
    {
        public string Name { get; set; }
        public Dictionary<string, JobConfiguration> Jobs { get; set; }  // parallel execution?
    }
}