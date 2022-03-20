using FluentValidation;

namespace CIPlatformWebApi.DataTransfer.User
{
    public class CreateUserDto
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotNull()
                .Matches("(^[a-zA-Z0-9]{1}([a-zA-Z0-9-_]{2,48})[a-zA-Z0-9]{1}$)")
                .WithMessage(
                    "Field `username` must be between 4 and 50 characters, may only contain alphanumeric characters, hyphens and underscores.");
            RuleFor(x => x.Name)
                .Length(1, 75)
                .WithMessage("Field `name` must be between 1 and 75 characters.");
            RuleFor(x => x.Email)
                .EmailAddress()
                .Length(1, 255)
                .WithMessage("Field `email` is invalid.");
            RuleFor(x => x.Password)
                .NotNull()
                .Length(8, 255)
                .WithMessage("Field `password` is invalid.");
        }
    }
}