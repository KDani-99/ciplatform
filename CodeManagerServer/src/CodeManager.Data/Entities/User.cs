using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeManager.Data.Models
{
    public class User : Entity
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
        [Column(TypeName = "Roles[]")]
        public Roles [] Roles { get; set; }
        public bool IsActive { get; set; }
        public string Password { get; set; }

        [ForeignKey(nameof(PlanId))]
        public long PlanId { get; set; }
        public Plan Plan { get; set; }
        
        public List<Team> Teams { get; set; }
        public DateTime RegistrationTimestamp { get; set; }
        public string RefreshTokenSignature { get; set; }
        public List<LoginHistory> LoginHistory { get; set; } = new();
    }
}