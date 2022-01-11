﻿namespace CodeManagerWebApi.Entities.Configuration
{
    public class JwtConfiguration
    {
        public int LifeTimeMinutes { get; set; }
        public string Secret { get; set; }
    }
}