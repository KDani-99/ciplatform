﻿using System.Security.Claims;
using System.Threading.Tasks;
using CodeManager.Data.Entities;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Entities;

namespace CodeManagerWebApi.Services
{
    public interface ITokenService<T>
    {
        public Task<T> CreateAccessTokenAsync(User user);
        public Task<T> CreateRefreshTokenAsync(User user);
        public Task InvalidateAccessTokenAsync(string username);
        public Task InvalidRefreshTokenAsync(string username);
        public Task<ClaimsPrincipal> VerifyRefreshTokenAsync(string token);

        public Task<ClaimsPrincipal> VerifyAccessTokenAsync(string token);
    }
}