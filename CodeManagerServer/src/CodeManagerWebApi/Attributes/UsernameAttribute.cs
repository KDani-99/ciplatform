using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CodeManagerWebApi.Attributes
{
    public class UsernameAttribute : ValidationAttribute
    {
        private const string ValidationRegexPattern =
            "(^[a-zA-Z0-9]{1}([a-zA-Z0-9-_]{2,48})[a-zA-Z0-9]{1}$)";
        
        public UsernameAttribute() : base("") {}

        public override bool IsValid(object value)
        {
            var password = value as string;

            if (password is null)
            {
                return false;
            }

            return new Regex(ValidationRegexPattern).IsMatch(password);
        }
    }
}