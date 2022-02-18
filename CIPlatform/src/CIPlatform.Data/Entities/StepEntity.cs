using System;
using CIPlatform.Data.Configuration;

namespace CIPlatform.Data.Entities
{
    public class Step : Entity
    {
        public string Name { get; set; }
        public int Index { get; set; }
        public States State { get; set; }
        public string LogPath { get; set; }
        public JobEntity Job { get; set; }
        public DateTime? StartedDateTime { get; set; }
        public DateTime? FinishedDateTime { get; set; }
    }
}