using System.ComponentModel.DataAnnotations;
using EmailValidation;

namespace CodeManagerWebApi.Attributes
{
    public class EmailAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return EmailValidator.Validate(value as string);
        }
    }
}