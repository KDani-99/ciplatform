using System;

namespace CodeManagerWebApi.Entities
{
    public class LoginHistory
    {
        public DateTime Timestamp { get; set; }
        
        public string UserAgent { get; set; }
        
        public string IP { get; set; }
    }
}