using System;

namespace CIPlatformWebApi.DataTransfer.User
{
    public class UserDto
    {
        public long Id { get; set; }
        public int Teams { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime Registration { get; set; }
        
        protected bool Equals(UserDto other)
        {
            return Id == other.Id && Teams == other.Teams && Name == other.Name && Username == other.Username &&
                Email == other.Email && IsAdmin == other.IsAdmin && Registration.Equals(other.Registration);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((UserDto) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Teams, Name, Username, Email, IsAdmin, Registration);
        }
    }
}