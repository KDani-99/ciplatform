using System;
using CodeManager.Data.Entities;

namespace CodeManagerWebApi.DataTransfer
{
    public class TeamMemberDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Username { get; set; }
        public Permissions Permission { get; set; }
        public DateTime JoinTime { get; set; }
    }
}