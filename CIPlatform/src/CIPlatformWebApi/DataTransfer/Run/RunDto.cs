using System;
using CIPlatform.Data.Configuration;

namespace CIPlatformWebApi.DataTransfer
{
    public class RunDto
    {
        public long Id { get; set; }
        public DateTime? StartedDateTime { get; set; }
        public DateTime? FinishedDateTime { get; set; }
        public States State { get; set; }
        public int Jobs { get; set; }
    }
}