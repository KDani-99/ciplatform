using FluentValidation;

namespace CIPlatformWebApi.DataTransfer
{
    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotNull()
                .Matches("(^[a-zA-Z0-9]{1}([a-zA-Z0-9-_]{2,48})[a-zA-Z0-9]{1}$)")
                .WithMessage("Field `username` must be between 4 and 50 characters.");
            RuleFor(x => x.Password)
                .NotNull()
                .Length(8, 255)
                .WithMessage("Field `password` is invalid.");
        }
    }
}