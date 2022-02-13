using System;
using CodeManager.Data.Configuration;

namespace CodeManager.Data.Entities
{
    public class ProcessedJobResultEvent
    {
        public long RunId { get; set; }
        public long JobId { get; set; }
        public States State { get; set; }
        public DateTime? DateTime { get; set; }
    }
}