using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CodeManagerWebApi.Entities
{
    public class User : Entity
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
        public IEnumerable<Roles> Roles { get; set; }
        public bool IsActive { get; set; }
        public string Password { get; set; }

        public Plan Plan { get; set; }
        
        public List<Team> Teams { get; set; }
        public DateTime RegistrationTimestamp { get; set; }
        public string RefreshTokenSignature { get; set; } // store only the signature of the request token, not the entire token
        // Permission group??
        public IList<LoginHistory> LoginHistory { get; set; } = new List<LoginHistory>();
    }
}