using System.Collections.Generic;

namespace CodeManagerWebApi.Entities.CI
{
    public class Run : Entity
    {
        public string ContextFilePath { get; set; }
        public List<Job> Jobs { get; set; }
    }
}