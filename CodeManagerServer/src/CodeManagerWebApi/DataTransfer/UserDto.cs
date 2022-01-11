using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CodeManagerWebApi.Attributes;

namespace CodeManagerWebApi.DataTransfer
{
    public record UserDto
    {
        public string Id { get; init; }
        [Required]
        [Username]
        public string Username { get; init; }
        [Required]
        [Name]
        public string Name { get; init; }
        [Required]
        [EmailAddress]
        public string Email { get; init; }
        [Required]
        [Password]
        public string Password { get; init; }
        
    }
}
