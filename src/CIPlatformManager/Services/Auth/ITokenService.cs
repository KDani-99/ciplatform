﻿using System.Security.Claims;
using System.Threading.Tasks;

namespace CIPlatformManager.Services.Auth
{
    public interface ITokenService<T>
    {
        public Task<T> CreateJobTokenAsync(long runId, long jobId);
        public Task<ClaimsPrincipal> VerifyJobTokenAsync(string token);
    }
}