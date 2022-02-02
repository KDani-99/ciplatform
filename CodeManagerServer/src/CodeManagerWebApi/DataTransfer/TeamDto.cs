﻿using FluentValidation;

namespace CodeManagerWebApi.DataTransfer
{
    public class TeamDto
    {
        public long Id { get; set; }
        public long Members { get; set; }
        public long Projects { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public bool IsPublic { get; set; }
        public string Owner { get; set; }
    }
    
    public class TeamDtoValidator : AbstractValidator<TeamDto> {
        public TeamDtoValidator() {
            RuleFor(x => x.Name)
                .NotNull()
                .Length(1, 50)
                .WithMessage("Field `name` must be between 1 and 50 characters.");
            RuleFor(x => x.Description)
                .NotNull()
                .Length(0, 150)
                .WithMessage("Field `Description` must be between 1 and 150 characters.");
            RuleFor(x => x.Image)
                .NotNull()
                .Length(0, 255)
                .WithMessage("Field `Image` must be between 1 and 255 characters.");
        }
    }
}