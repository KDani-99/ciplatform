using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CIPlatform.Data.Entities
{
    public class User : Entity
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [Column(TypeName = "Roles[]")] public Roles[] Roles { get; set; }
        public string Password { get; set; }
        public List<TeamEntity> Teams { get; set; } = new();
        public DateTime RegistrationTimestamp { get; set; }
    }
}