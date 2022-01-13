﻿using System;

namespace CodeManagerWebApi.Entities
{
    public class LoginHistory : Entity
    {
        public DateTime Timestamp { get; set; }
        
        public string UserAgent { get; set; }
        
        public string IP { get; set; }
        
        public User User { get; set; }
    }
}