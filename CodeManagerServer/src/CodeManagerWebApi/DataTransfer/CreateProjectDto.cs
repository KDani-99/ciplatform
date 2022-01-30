using System;
using System.Collections.Generic;
using CodeManager.Data.Entities;
using CodeManager.Data.Entities.CI;
using FluentValidation;

namespace CodeManagerWebApi.DataTransfer
{
    public class CreateProjectDto
    {
        public string RepositoryUrl { get; set; }
        public bool IsPrivateRepository { get; set; }
        public bool IsPrivateProject { get; set; }
        public bool IsSSH { get; set; }
        public string Username { get; set; }
        public string SecretToken { get; set; } // Either SSH or access token
        public long TeamId { get; set; } // owner team id
    }
    
    public class CreateProjectDtoValidator : AbstractValidator<CreateProjectDto> {
        public CreateProjectDtoValidator() {
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
            RuleFor(x => x.IsSSH)
                .NotNull()
                .WithMessage("Field `IsSSH` is invalid.");
            When(x => x.IsPrivateProject, () =>
            {
                RuleFor(x => x.SecretToken)
                    .NotNull()
                    .Length(1, 255)
                    .WithMessage("Field `SecretToken` is required if the project is private.");

                When(x => !x.IsSSH, () =>
                {
                    RuleFor(x => x.Username)
                        .NotNull()
                        .Length(1, 255)
                        .WithMessage("Field `Username` is required if not using ssh.");
                })
                .Otherwise(() =>
                {
                    RuleFor(x => x.SecretToken)
                        .NotNull()
                        .Length(1, 255)
                        .WithMessage("Field `SecretToken` is required if the project is private (SSH key).");
                });;
            });
            RuleFor(x => x.TeamId)
                .NotNull()
                .WithMessage("Field `TeamId` is invalid.");
        }
    }
}