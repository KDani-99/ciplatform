using FluentValidation;

namespace CIPlatformWebApi.DataTransfer.Project
{
    public class CreateProjectDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string RepositoryUrl { get; set; }
        public bool IsPrivateProject { get; set; }
        public long TeamId { get; set; } // owner team id
    }

    public class CreateProjectDtoValidator : AbstractValidator<CreateProjectDto>
    {
        public CreateProjectDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotNull()
                .Length(1, 50)
                .WithMessage("Field `Name` must be between 1 and 50 characters.");
            RuleFor(x => x.Description)
                .NotNull()
                .Length(1, 150)
                .WithMessage("Field `Description` must be between 1 and 150 characters.");
            RuleFor(x => x.RepositoryUrl)
                .NotNull()
                .Length(1, 255)
                .WithMessage("Field `RepositoryUrl` must be between 1 and 255 characters.");
            RuleFor(x => x.IsPrivateProject)
                .NotNull()
                .WithMessage("Field `IsPrivateProject` is invalid.");
            RuleFor(x => x.TeamId)
                .NotNull()
                .NotEmpty()
                .WithMessage("Field `TeamId` is invalid.");
        }
    }
}