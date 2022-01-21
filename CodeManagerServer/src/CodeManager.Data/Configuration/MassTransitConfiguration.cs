using System.Collections;
using System.Collections.Generic;

namespace CodeManager.Data.Configuration
{
    public class MassTransitConfiguration
    {
        public string Host { get; set; }
        public string VirtualHost { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Dictionary<string, string> Queues { get; set; }
    }
}