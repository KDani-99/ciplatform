﻿using CIPlatform.Data.Configuration;

namespace CIPlatform.Data.Events
{
    public class StepResultEvent
    {
        public string Token { get; set; } // JWT Token with relevant info like AgentID and JobID, RunID
        public States State { get; set; }
        public int StepIndex { get; set; }
    }
}