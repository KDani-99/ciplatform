using System;
using CIPlatform.Data.Agent;
using CIPlatform.Data.Configuration;

namespace CIPlatform.Data.Entities
{
    public class WorkerConnectionDataEntity
    {
        public string ConnectionId { get; set; }
        public JobContext JobContext { get; set; }
        public WorkerState WorkerState { get; set; }
        public DateTime LastPing { get; set; }
    }
}