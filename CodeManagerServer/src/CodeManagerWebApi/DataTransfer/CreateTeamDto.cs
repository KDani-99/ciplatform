using FluentValidation;

namespace CodeManagerWebApi.DataTransfer
{
    public class CreateTeamDto
    {
        public string Name { get; set; }
        public string Image { get; set; }
    }
    
    public class CreateTeamDtoValidator : AbstractValidator<CreateTeamDto> {
        public CreateTeamDtoValidator() {
            RuleFor(x => x.Name)
                .NotNull()
                .Length(1, 50)
                .WithMessage("Field `name` must be between 1 and 50 characters.");
        }
    }
}