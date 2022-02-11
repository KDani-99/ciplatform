using System;

namespace CodeManagerWebApi.DataTransfer
{
    public class UserDto
    {
        public long Id { get; set; }
        public int Teams { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public string Plan { get; set; }
        public string Image { get; set; }
        public DateTime Registration { get; set; }
    }
}