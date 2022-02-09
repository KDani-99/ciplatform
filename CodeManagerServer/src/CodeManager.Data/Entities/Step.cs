using System;
using CodeManager.Data.Configuration;
using CodeManager.Data.Entities;

namespace CodeManager.Data.Entities.CI
{
    public class Step : Entity
    {
        public string Name { get; set; }
        public States State { get; set; }
        public string LogPath { get; set; }
        public Job Job { get; set; }
        public DateTime StartedDateTime { get; set; }
        public DateTime FinishedDateTime { get; set; }
    }
}