using System;
using CIPlatform.Data.Configuration;

namespace CIPlatform.Data.Entities
{
    public class ProcessedJobResultEvent
    {
        public long RunId { get; set; }
        public long JobId { get; set; }
        public States State { get; set; }
        public DateTime? DateTime { get; set; }
    }
}