﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace CodeManagerWebApi.DataTransfer
{
    public class UserDto
    {
        public string Id { get; init; }
        public string Username { get; init; }
        public string Name { get; init; }
        public string Email { get; init; }
        public string Password { get; init; }
        
    }
    
    public class UserDtoValidator : AbstractValidator<UserDto> {
        public UserDtoValidator() {
            RuleFor(x => x.Username)
                .NotNull()
                .Matches("(^[a-zA-Z0-9]{1}([a-zA-Z0-9-_]{2,48})[a-zA-Z0-9]{1}$)")
                .WithMessage("Field `username` must be between 4 and 50 characters.");
            RuleFor(x => x.Name)
                .Length(1, 75)
                .WithMessage("Field `name` must be between 1 and 75 characters.");
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("Field `email` is invalid.");
            RuleFor(x => x.Password)
                .NotNull()
                .Length(8, 255)
                .WithMessage("Field `password` is invalid.");
        }
    }
}
