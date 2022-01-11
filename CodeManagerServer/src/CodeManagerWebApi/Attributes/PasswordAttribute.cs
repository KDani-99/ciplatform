using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using CodeManagerWebApi.DataTransfer;
using Microsoft.AspNetCore.Identity;

namespace CodeManagerWebApi.Attributes
{
    public class PasswordAttribute : ValidationAttribute
    {
        private const string ValidationRegexPattern =
            "^(?=[^A-Z\n]*[A-Z])(?=[^a-z\n]*[a-z])(?=[^0-9\n]*[0-9])(?=[^#?!@$%^&*\n-]*[#?!@$%^&*-]).{6,}$";

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