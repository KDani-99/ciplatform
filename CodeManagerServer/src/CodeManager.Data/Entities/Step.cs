using System;
using CodeManager.Data.Configuration;

namespace CodeManager.Data.Entities
{
    public class Step : Entity
    {
        public string Name { get; set; }
        public int Index { get; set; }
        public States State { get; set; }
        public string LogPath { get; set; }
        public Job Job { get; set; }
        public DateTime StartedDateTime { get; set; }
        public DateTime FinishedDateTime { get; set; }
    }
}