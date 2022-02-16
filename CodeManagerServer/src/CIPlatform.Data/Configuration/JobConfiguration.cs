using System.Collections.Generic;
using Docker.DotNet.Models;

namespace CIPlatform.Data.Configuration
{
    public class JobConfiguration
    {
        // Composition over inheritance
        public string Image { get; set; }
        public AuthConfig AuthConfig { get; set; }
        public List<string> Environment { get; set; }
        public List<StepConfiguration> Steps { get; set; }
        public JobContext Context { get; set; }
    }
}