using System;
using CodeManager.Data.Configuration;

namespace CodeManager.Data.Entities
{
    public class ProcessedRunResultEvent
    {
        public long ProjectId { get; set; }
        public long RunId { get; set; }
        public States State { get; set; }
        public DateTime? DateTime { get; set; }
    }
}