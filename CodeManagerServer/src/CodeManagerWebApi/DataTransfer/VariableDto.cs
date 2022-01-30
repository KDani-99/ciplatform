using FluentValidation;

namespace CodeManagerWebApi.DataTransfer
{
    public class VariableDto
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsSecret { get; set; }
    }
    
    public class VariableDtoValidator : AbstractValidator<VariableDto> {
        public VariableDtoValidator() {
            RuleFor(x => x.Name)
                .NotNull()
                .Matches("^([a-zA-Z0-9_-]){1,50}$")
                .WithMessage("Field `name` is invalid.");
            RuleFor(x => x.Value)
                .Length(1, 255)
                .WithMessage("Field `Value` must be between 1 and 255 characters.");
            RuleFor(x => x.IsSecret)
                .NotNull()
                .WithMessage("Field `IsSecret` is invalid.");
        }
    }
}