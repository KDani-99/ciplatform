using System.Collections.Generic;

namespace CodeManagerWebApi.Entities.CI
{
    public class Job : Entity
    {
        public string Name { get; set; }
        public List<Step> Steps { get; set; }
    }
}