using System;
using System.Collections.Generic;
using CodeManager.Data.Entities;
using CodeManager.Data.Entities.CI;
using FluentValidation;

namespace CodeManagerWebApi.DataTransfer
{
    public class CreateProjectDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string RepositoryUrl { get; set; }
        public bool IsPrivateRepository { get; set; }
        public bool IsPrivateProject { get; set; }
        public string Username { get; set; }
        public string SecretToken { get; set; } // Either SSH or access token
        public long TeamId { get; set; } // owner team id
    }
    
    public class CreateProjectDtoValidator : AbstractValidator<CreateProjectDto> {
        public CreateProjectDtoValidator() {
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
            RuleFor(x => x.IsPrivateRepository)
                .NotNull()
                .WithMessage("Field `IsPrivateRepository` is invalid.");
            RuleFor(x => x.IsPrivateProject)
                .NotNull()
                .WithMessage("Field `IsPrivateProject` is invalid.");
            When(x => x.IsPrivateRepository, () =>
            {
                RuleFor(x => x.SecretToken)
                    .NotNull()
                    .Length(1, 255)
                    .WithMessage("Field `SecretToken` is required if the repository is private.");

                RuleFor(x => x.Username)
                    .NotNull()
                    .Length(1, 100)
                    .WithMessage("Field `Username` is required if the repository is private.");
            });
            RuleFor(x => x.TeamId)
                .NotNull()
                .NotEmpty()
                .WithMessage("Field `TeamId` is invalid.");
        }
    }
}