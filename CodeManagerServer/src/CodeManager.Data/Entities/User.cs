using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeManager.Data.Entities
{
    public class User : Entity
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
        [Column(TypeName = "Roles[]")]
        public Roles [] Roles { get; set; }
        public string Password { get; set; }

        [ForeignKey(nameof(PlanId))]
        public long PlanId { get; set; } // TODO: remove
        public Plan Plan { get; set; }

        public List<Team> Teams { get; set; } = new();
        public DateTime RegistrationTimestamp { get; set; }
        public string RefreshTokenSignature { get; set; }
        public List<LoginHistory> LoginHistory { get; set; } = new();
    }
}