using System.ComponentModel.DataAnnotations;

namespace CodeManagerWebApi.Attributes
{
    public class NameAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return true;
        }
    }
}