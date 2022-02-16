using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CIPlatform.Data.Configuration
{
    [DataContract]
    public class MassTransitConfiguration
    {
        public string Host { get; set; }
        public string VirtualHost { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public IDictionary<string, string> Queues { get; set; }
    }
}