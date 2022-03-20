using System.Collections.Generic;

namespace CIPlatform.Data.Configuration
{
    public class RunConfiguration
    {
        public Dictionary<string, JobConfiguration> Jobs { get; set; }
    }
}