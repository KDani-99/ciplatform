using System.Collections.Generic;

namespace CodeManagerShared.Configuration
{
    public class Job
    {
        public string Image { get; set; }
        public List<string> Environment { get; set; }
        public List<Step> Steps { get; set; }
    }
}