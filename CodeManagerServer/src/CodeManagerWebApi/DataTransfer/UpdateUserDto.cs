using FluentValidation;

namespace CodeManagerWebApi.DataTransfer
{
    public class UpdateUserDto
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
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
            When(x => x.Password is {Length: > 0}, () =>
            {
                RuleFor(x => x.Password)
                    .Length(8, 255)
                    .WithMessage("Field `password` is invalid.");
            });
        }
    }
}