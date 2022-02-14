﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManagerWebApi.DataTransfer;
using Microsoft.AspNetCore.Http;

namespace CodeManagerWebApi.Services
{
    public interface IUserService
    {
        public Task CreateUser(CreateUserDto createUserDto);

        public Task<UserDto> GetUserAsync(long id, User user);
        public Task<IEnumerable<UserDto>> GetUsersAsync(User user);

        public Task<AuthTokenDto> LoginAsync(LoginDto userDto, HttpContext httpContext);
        public Task<AuthTokenDto> GenerateAuthTokensAsync(string username);
        public Task UpdateUserAsync(long id, UpdateUserDto updateUserDto, User user);

        public Task DeleteUserAsync(long id, User user);
    }
}